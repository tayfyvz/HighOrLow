using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers;
using Managers.Managers;
using TMPro;
using UnityEngine;
using Utils.AddressableLoaders;
using Logger = Utils.Logger;

namespace Views
{
    public class BetView : MonoBehaviour, IBetView, IView
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _awardedScoreText;
        [SerializeField] private TextMeshProUGUI _comboMultiplierText;

        [Header("GameObject References")]
        [SerializeField] private GameObject _comboMultiplierObj;
        [SerializeField] private GameObject _awardedScoreObj;

        [Header("Transform References")]
        [SerializeField] private Transform _comboMultiplierTransform;
        [SerializeField] private Transform _awardedScoreTransform;
        [SerializeField] private Transform _scoreTransform;

        [Header("CanvasGroup")]
        [SerializeField] private CanvasGroup _awardedScoreCanvasGroup;

        [Header("Animation Settings")]
        [SerializeField] private float _scoreChangeDuration = 0.5f;
        [SerializeField] private float _comboMultiplierPunchDuration = 0.3f;
        [SerializeField] private float _awardedScoreMovementDuration = 0.3f;
        [SerializeField] private float _betButtonMovementDuration = 0.3f;

        [SerializeField] private Ease _awardedScoreMovementEase = Ease.OutBounce;
        [SerializeField] private Vector3 _comboMultiplierPunchVector = new(0.5f, 0.5f, 0);

        private List<BetButtonView> _betButtons;
        private int _currentScore;
        private BetButtonLoader _betButtonLoader;
        private Sequence _sequence;

        private const string PLUS = "+";
        private const string CROSS = "x";
        private const string LOSE_BET = "Bet Lost!";

        private void Start()
        {
            _betButtonLoader = new BetButtonLoader();
            _currentScore = 0;
        }

        private void OnEnable() => ResetView();

        public async UniTask InstantiateBetButtons(Vector2[] playersPositions, Transform[] playersTransforms, Action<int> onBetButtonClicked)
        {
            _betButtons = new List<BetButtonView>();
            for (int i = 0; i < playersPositions.Length; i++)
            {
                BetButtonView betButtonView = await _betButtonLoader.LoadAsync();
                _betButtons.Add(betButtonView);
                if (betButtonView != null)
                {
                    betButtonView.buttonTransform.SetParent(playersTransforms[i], false);
                    betButtonView.buttonTransform.position = playersPositions[i];
                    int index = i;
                    betButtonView.betButton.onClick.AddListener(() => onBetButtonClicked(index));
                }
            }
        }

        public void UpdateScore(int score)
        {
            if (_scoreText == null) return;
            if (score > _currentScore)
            {
                DOTween.To(() => _currentScore, x =>
                {
                    _currentScore = x;
                    _scoreText.text = _currentScore.ToString();
                }, score, _scoreChangeDuration);
            }
            else
            {
                _scoreText.text = _currentScore.ToString();
            }
        }

        public void ResetView()
        {
            ResetTexts();
            ResetGameObjects();
        }

        private void ResetTexts()
        {
            if (_scoreText != null) _scoreText.text = "0";
            if (_awardedScoreText != null) _awardedScoreText.text = string.Empty;
            if (_comboMultiplierText != null) _comboMultiplierText.text = string.Empty;
        }

        private void ResetGameObjects()
        {
            if (_comboMultiplierObj != null) _comboMultiplierObj.SetActive(false);
            if (_awardedScoreObj != null) _awardedScoreObj.SetActive(false);
        }

        public async UniTask PlayWinBetAnimSeq(int comboMultiplier, int awardedPoints, CancellationToken cancellationToken)
        {
            if (_sequence != null && _sequence.IsActive()) _sequence.Kill();
            _comboMultiplierObj.SetActive(true);
            _comboMultiplierText.text = comboMultiplier + CROSS;
            _awardedScoreObj.SetActive(true);
            _awardedScoreText.text = PLUS + awardedPoints;
            _awardedScoreText.color = Color.green;
            Vector2 originalPos = _awardedScoreTransform.position;

            _sequence = DOTween.Sequence();
            _sequence.Append(_comboMultiplierTransform.DOPunchScale(_comboMultiplierPunchVector, _comboMultiplierPunchDuration, 5, 1));
            _sequence.Join(_awardedScoreTransform.DOMove(_scoreTransform.position, _awardedScoreMovementDuration).SetEase(_awardedScoreMovementEase));
            _sequence.Join(_awardedScoreCanvasGroup.DOFade(0f, _awardedScoreMovementDuration));
            _sequence.JoinCallback(() => IncreaseSound(comboMultiplier));

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

            _awardedScoreObj.SetActive(false);
            _awardedScoreTransform.position = originalPos;
            _awardedScoreCanvasGroup.alpha = 1f;
        }

        public async UniTask PlayLoseBetAnimSeq(int comboMultiplier, CancellationToken cancellationToken)
        {
            if (_sequence != null && _sequence.IsActive()) _sequence.Kill();
            _awardedScoreObj.SetActive(true);
            _awardedScoreText.text = LOSE_BET;
            _awardedScoreText.color = Color.red;
            Vector2 originalPos = _awardedScoreTransform.position;

            _sequence = DOTween.Sequence();
            _sequence.Append(_awardedScoreTransform.DOMove(_scoreTransform.position, _awardedScoreMovementDuration).SetEase(Ease.OutBounce));
            _sequence.Join(_awardedScoreCanvasGroup.DOFade(0f, _awardedScoreMovementDuration));
            _sequence.JoinCallback(DecreaseSound);

            if (_comboMultiplierObj.activeSelf)
            {
                _sequence.Join(_comboMultiplierTransform.DOScale(Vector3.zero, 0.3f));
                _sequence.OnComplete(() =>
                {
                    _comboMultiplierObj.SetActive(false);
                });
            }
            
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

            _awardedScoreObj.SetActive(false);
            _awardedScoreTransform.position = originalPos;
            _awardedScoreCanvasGroup.alpha = 1f;
            
            _comboMultiplierTransform.localScale = Vector3.one;
            _comboMultiplierObj.SetActive(false); 
        }

        public void ResetBet()
        {
            foreach (BetButtonView buttonView in _betButtons)
            {
                Transform buttonTransform = buttonView.buttonTransform;

                DOTween.Sequence()
                    .Append(buttonTransform.DOScale(Vector3.one, _betButtonMovementDuration))
                    .Join(buttonView.canvasGroup.DOFade(1f, _betButtonMovementDuration))
                    .OnComplete(() =>
                    {
                        if (buttonView != null)
                        {
                            buttonView.SetInteractable(true);
                            buttonView.SetActive(true);
                        }
                    });
            }
        }


        public void DeactivateButtonsExcept(int playerIndex)
        {
            for (int i = 0; i < _betButtons.Count; i++)
            {
                int index = i;
                _betButtons[index].SetInteractable(false);
        
                if (index != playerIndex)
                {
                    Transform buttonTransform = _betButtons[index].buttonTransform;

                    DOTween.Sequence()
                        .Append(buttonTransform.DOScale(Vector3.zero, _betButtonMovementDuration))
                        .Join(_betButtons[index].canvasGroup.DOFade(0f, _betButtonMovementDuration))
                        .OnComplete(() =>
                        {
                            if (index >= 0 && index < _betButtons.Count)
                            {
                                _betButtons[index].SetActive(false);
                            }
                        });
                }
            }
        }


        public void DeactivateButtons()
        {
            foreach (BetButtonView buttonView in _betButtons)
            {
                buttonView.SetInteractable(false);
                Transform buttonTransform = buttonView.buttonTransform;
                    
                DOTween.Sequence()
                    .Append(buttonTransform.DOScale(Vector3.zero, _betButtonMovementDuration))
                    .Join(buttonView.canvasGroup.DOFade(0f, _betButtonMovementDuration))
                    .OnComplete(() => buttonView.gameObject.SetActive(false));
            }
        }

        private void IncreaseSound(int comboMultiplier)
        {
            SoundManager.Instance.PlaySound(SoundType.BetWin);
            if (AudioManager.Instance.musicSource != null)
                AudioManager.Instance.musicSource.pitch = 1f + (comboMultiplier / 10f);
        }

        private void DecreaseSound()
        {
            SoundManager.Instance.PlaySound(SoundType.BetLose);
            if (AudioManager.Instance.musicSource != null)
                AudioManager.Instance.musicSource.pitch = 1f;
        }

        private void OnDestroy()
        {
            if (_betButtonLoader == null) return;
            _betButtonLoader.Release();
            _betButtonLoader = null;
        }
    }
}
