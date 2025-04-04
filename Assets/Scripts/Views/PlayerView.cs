using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers.Managers;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private float _scoreTextScaleDuration = 0.2f;
        [SerializeField] private float _scoreTextReScaleDuration = 0.3f;
        [SerializeField] private float _scoreTextColorDur = 0.2f;
        [SerializeField] private float _imageScaleDuration = 0.3f;
        [SerializeField] private float _imageReScaleDuration = 0.2f;
        
        [SerializeField] private Color _textOriginalColor;
        [SerializeField] private Color _textWinColor;

        [SerializeField] private Vector3 PosRotate = new Vector3(0, 0, 30f);
        [SerializeField] private Vector3 NegRotate = new Vector3(0, 0, -30f);
        [SerializeField] private Vector3 _playerImageRotate = new Vector3(0, 720, 0);
        
        [SerializeField] private Ease _playerImageRotateEase = Ease.OutQuad;
        [SerializeField] private Ease _playerImageScaleEase = Ease.InBounce;
        [SerializeField] private Ease _scoreTextScaleEase = Ease.OutBack;
        [SerializeField] private Ease _imageScaleEase = Ease.OutBounce;
        
        private Transform _transform;
        private string _text;
        
        private const string WinText = "Win!";
        private static readonly Vector3 ScoreScaleVector = (1.5f * Vector3.one);
        private static readonly Vector3 ImageScaleVector = (1.3f * Vector3.one);

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
            Sequence winSequence = DOTween.Sequence();

            winSequence.Append(_scoreTextTransform.DOScale(ScoreScaleVector, _scoreTextScaleDuration).SetEase(_scoreTextScaleEase));
            winSequence.Join(_scoreText.DOColor(_textWinColor, _scoreTextColorDur));

            winSequence.Join(_playerImageTransform.DOScale(ImageScaleVector, _imageScaleDuration).SetEase(_imageScaleEase));
            winSequence.Append(_playerImageTransform.DOScale(Vector3.one, _imageReScaleDuration).SetEase(Ease.InQuad));

            winSequence.Append(_scoreTextTransform.DORotate(PosRotate, 0.2f).SetEase(Ease.OutCubic));
            winSequence.JoinCallback((() => SoundManager.Instance.PlaySound(SoundType.RoundEnd)));
            winSequence.Append(_scoreTextTransform.DORotate(NegRotate, 0.2f).SetEase(Ease.InCubic));
            winSequence.Append(_scoreTextTransform.DORotate(Vector3.zero, 0.1f));

            winSequence.Append(_scoreTextTransform.DOScale(Vector3.one, _scoreTextReScaleDuration).SetEase(Ease.OutBounce));
            winSequence.Append(_scoreText.DOColor(_textOriginalColor, _scoreTextReScaleDuration));

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
        
        public async UniTask PlayWinSessionAnim()
        {
            SoundManager.Instance.PlaySound(SoundType.GameEnd);
            Sequence winSequence = DOTween.Sequence();
            winSequence.Append(_playerImageTransform.DORotate(_playerImageRotate, 1f, RotateMode.FastBeyond360).SetEase(_playerImageRotateEase));
            winSequence.Append(_playerImageTransform.DOScale(ScoreScaleVector, 0.5f).SetEase(_imageScaleEase));
            winSequence.Append(_playerImageTransform.DOScale(Vector3.one, _imageScaleDuration).SetEase(_playerImageScaleEase));

            if (_scoreText != null)
            {
                _scoreText.text = WinText;
                _scoreText.DOColor(_textWinColor, _scoreTextReScaleDuration).SetLoops(2, LoopType.Yoyo);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(2f));
        }

        public Transform GetTransform() => _transform ??= transform;

        public void SetSprite(Sprite playerSprite)
        {
            if (_playerImage != null) _playerImage.sprite = playerSprite;
        }
    }
}
