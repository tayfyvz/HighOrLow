using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers.Managers;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using Utils.AddressableLoaders;
using Logger = Utils.Logger;

namespace Views
{
    public class DeckView : MonoBehaviour, IDeckView, IView
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _remainingCardsText;
        [SerializeField] private TextMeshProUGUI _drawCardAmountText;

        [Header("GameObjects References")]
        [SerializeField] private GameObject _drawCardAmountObj;

        [Header("Transform References")]
        [SerializeField] private Transform _remainingCardsTransform;
        [SerializeField] private Transform _drawCardAmountTransform;
        [SerializeField] private Transform _deckCardsTransform;
        [SerializeField] private Transform _discartCardsTransform;

        [Header("Animation Settings")]
        [SerializeField] private float _remainingCardsChangeDuration = 1f;
        [SerializeField] private float _drawCardAmountMovementDuration = 1f;
        [SerializeField] private float _cardMoveDuration = 0.5f;
        [SerializeField] private float _cardRotateDuration = 1f;
        [SerializeField] private float _cardDiscardDuration = 1.5f;
        [SerializeField] private float _cardFlipDuration = 0.5f;

        [SerializeField] private Ease _drawCardAmountMovementEase = Ease.OutBounce;
        [SerializeField] private Ease _cardMoveEase = Ease.OutQuad;
        [SerializeField] private Ease _cardRotateEase = Ease.Linear;

        [SerializeField] private Vector3 _cardRotateVector = new(0, 0, 360);

        [Header("CanvasGroup")]
        [SerializeField] private CanvasGroup _drawCardAmountCanvasGroup;

        private ObjectPool<CardView> _cardPool;
        private CardPrefabLoader _cardPrefabLoader;
        private List<CardView> _activeCards = new List<CardView>();
        private int _currentCardCount;
        private CardAtlasLoader _cardAtlasLoader;
        private Sprite _cardBackSprite;
        private Sequence _sequence;
        private const string MINUS = "-";
        private readonly Vector3 _flipVector = new Vector3(0, 90, 0);

        private async void Awake()
        {
            _cardAtlasLoader = new CardAtlasLoader();
            _currentCardCount = 52;
            await _cardAtlasLoader.LoadAsync();
            _cardBackSprite = _cardAtlasLoader.GetCardBackSprite();
            _cardPrefabLoader = new CardPrefabLoader();
            _cardPool = new ObjectPool<CardView>(CreateCard, OnGetCard, OnReleaseCard, OnDestroyCard);

            for (int i = 0; i < 4; i++)
            {
                CardView card = await _cardPrefabLoader.LoadAsync();
                card.transform.SetParent(_deckCardsTransform);
                card.SetBackSprite(_cardBackSprite);
                _cardPool.Release(card);
            }

            ResetView();
        }

        private void OnDestroy()
        {
            _cardAtlasLoader?.Release();
            _cardAtlasLoader = null;
            _cardPrefabLoader?.Release();
            _cardPrefabLoader = null;
            _cardPool.Dispose();
        }

        public void UpdateDeckCount(int count)
        {
            if (_remainingCardsText == null) return;
            if (count <= _currentCardCount)
            {
                DOTween.To(() => _currentCardCount, x =>
                {
                    _currentCardCount = x;
                    _remainingCardsText.text = _currentCardCount.ToString();
                }, count, _remainingCardsChangeDuration);
            }
            else
            {
                _currentCardCount = count;
                _remainingCardsText.text = _currentCardCount.ToString();
            }
        }

        public void PlayDrawCardAmountAnim(int amount)
        {
            _activeCards.Clear();
            _drawCardAmountObj.SetActive(true);
            _drawCardAmountText.text = MINUS + amount;
            Vector2 originalPos = _drawCardAmountTransform.position;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_drawCardAmountTransform.DOMove(_remainingCardsTransform.position, _drawCardAmountMovementDuration).SetEase(_drawCardAmountMovementEase));
            sequence.Join(_drawCardAmountCanvasGroup.DOFade(0f, _drawCardAmountMovementDuration));
            sequence.OnComplete(() =>
            {
                _drawCardAmountObj.SetActive(false);
                _drawCardAmountTransform.position = originalPos;
                _drawCardAmountCanvasGroup.alpha = 1f;
            });
        }

        public async UniTask PlayDistributeCardAnim(Card roundCard, Vector2 destination, CancellationToken cancellationToken)
        {
            if (_cardAtlasLoader == null || roundCard == null) return;
            if (_sequence != null && _sequence.IsActive()) _sequence.Kill();

            CardView card = _cardPool.Get();
            card.SetPosition(_deckCardsTransform.position);
            _activeCards.Add(card);

            Sprite cardSprite = _cardAtlasLoader.GetCardSprite(roundCard);
            card.SetFrontSprite(cardSprite);
            card.ShowBack();

            _sequence = DOTween.Sequence();
            _sequence.Append(card.transform.DOMove(destination, _cardMoveDuration).SetEase(_cardMoveEase));
            _sequence.Join(card.transform.DORotate(_cardRotateVector, _cardRotateDuration, RotateMode.FastBeyond360).SetEase(_cardRotateEase));
            _sequence.AppendCallback(() => SoundManager.Instance.PlaySound(SoundType.DrawCard));

            var completionSource = new UniTaskCompletionSource();
            _sequence.OnComplete(() =>
            {
                Logger.Log("Animation completed.", LogType.Log);
                completionSource.TrySetResult();
            });

            try
            {
                await completionSource.Task.AttachExternalCancellation(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Logger.Log("Animation was canceled.", LogType.Warning);
            }
        }

        public async UniTask PlayFlipCardAnim()
        {
            foreach (CardView card in _activeCards)
            {
                _sequence = DOTween.Sequence();
                _sequence.Append(card.transform.DORotate(_flipVector, _cardFlipDuration, RotateMode.Fast).SetEase(Ease.Linear));
                _sequence.AppendCallback(card.ShowFront);
                _sequence.Append(card.transform.DORotate(Vector3.zero, _cardFlipDuration, RotateMode.Fast).SetEase(Ease.Linear));
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_cardFlipDuration));
        }

        public async UniTask PlayDiscardCardsAnim()
        {
            foreach (CardView card in _activeCards)
            {
                card.transform.DOMove(_discartCardsTransform.position, _cardDiscardDuration).SetEase(_cardMoveEase)
                    .OnComplete(() => _cardPool.Release(card));
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_cardDiscardDuration));
        }

        public void ResetView()
        {
            ResetTexts();
            ResetGameObjects();
        }

        private void ResetTexts()
        {
            _remainingCardsText?.SetText(string.Empty);
            _drawCardAmountText?.SetText(string.Empty);
        }

        private void ResetGameObjects()
        {
            _drawCardAmountObj?.SetActive(false);
        }

        private CardView CreateCard() => _cardPrefabLoader.LoadAsync().GetAwaiter().GetResult();

        private void OnGetCard(CardView card) => card.SetActive(true);

        private void OnReleaseCard(CardView card) => card.SetActive(false);

        private void OnDestroyCard(CardView card) => Destroy(card.gameObject);
    }
}
