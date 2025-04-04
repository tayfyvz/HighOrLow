using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers.Managers;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Utils.Logger;

namespace Views
{
    public class PlayerView : MonoBehaviour, IPlayerView, IView
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _handText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Images")]
        [SerializeField] private Image _playerImage;

        [Header("GameObjects References")]
        [SerializeField] private GameObject _handTextObj;

        [Header("Transform References")]
        [SerializeField] private Transform _handTransform;
        [SerializeField] private Transform _buttonTransform;
        [SerializeField] private Transform _cardTransform;
        [SerializeField] private Transform _scoreTextTransform;
        [SerializeField] private Transform _playerImageTransform;
        [SerializeField] private Transform _leftHandTransform;
        [SerializeField] private Transform _rightHandTransform;

        [Header("Animations References")]
        [SerializeField] private float _textAnimationDuration = 0.1f;
        [SerializeField] private Color _textOriginalColor;
        [SerializeField] private Color _textWinColor;

        private Transform _transform;
        private string _text;

        private void Awake()
        {
            _transform = transform;
            _handTextObj.SetActive(false);
            _scoreText.color = _textOriginalColor;
        }

        public void SetPlayerName(string name)
        {
            if (_playerNameText != null) _playerNameText.text = name;
        }

        public void UpdateHand(Card card, int id)
        {
            _text = card.ToString();
            _handTransform.position = id % 2 == 0 ? _leftHandTransform.position : _rightHandTransform.position;
        }

        public void SetPosition(Vector2 position) => _transform.position = position;

        public void SetScore(int score)
        {
            if (_scoreText != null) _scoreText.text = score.ToString();
        }

        public void ResetView()
        {
            SetScore(0);
            _handText.text = string.Empty;
        }

        public Vector2 GetButtonPosition() => _buttonTransform.position;

        public Vector2 GetCardPosition() => _cardTransform.position;

        public async void PlayReadCardAnim()
        {
            _handTextObj.SetActive(true);
            _handText.text = string.Empty;

            foreach (char c in _text)
            {
                _handText.text += c;
                _handTransform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutBack);
                await UniTask.Delay(TimeSpan.FromSeconds(_textAnimationDuration));
                _handTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InQuad);
            }
        }

        public async UniTask PlayWinScoreAnim()
        {
            if (_scoreText == null)
            {
                Logger.Log("Score text is not set.", LogType.Warning);
                return;
            }

            SoundManager.Instance.PlaySound(SoundType.RoundEnd);
            _scoreText.DOColor(_textWinColor, 0.5f).OnComplete(() => _scoreText.DOColor(_textOriginalColor, 0.5f));
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }

        public async UniTask PlayWinSessionAnim()
        {
            if (_playerImage == null)
            {
                Logger.Log("Player image is not set.", LogType.Warning);
                return;
            }

            SoundManager.Instance.PlaySound(SoundType.GameEnd);
            _playerImageTransform.DORotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
            _playerImageTransform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.OutBounce);
            _playerImageTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBounce);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }

        public Transform GetTransform() => _transform ??= transform;

        public void SetSprite(Sprite playerSprite)
        {
            if (_playerImage != null) _playerImage.sprite = playerSprite;
        }
    }
}
