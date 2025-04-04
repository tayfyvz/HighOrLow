using System.Threading;
using Controllers;
using Cysharp.Threading.Tasks;
using Managers;
using Managers.Managers;
using UnityEngine;
using UnityEngine.UI;
using Logger = Utils.Logger;

namespace Views
{
    public class GameSessionManager : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button _drawCardButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _settingsButton;

        [Header("GameObject References")]
        [SerializeField] private GameObject _drawCardButtonObj;
        [SerializeField] private GameObject _newGameButtonObj;

        private IGameController _gameController;
        private readonly SemaphoreSlim _roundLock = new SemaphoreSlim(1, 1);

        private void Start()
        {
            if (_drawCardButtonObj == null && _drawCardButton != null) _drawCardButtonObj = _drawCardButton.gameObject;
            if (_newGameButtonObj == null && _newGameButton != null) _newGameButtonObj = _newGameButton.gameObject;
            _settingsButton?.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            _drawCardButton.onClick.AddListener(OnDrawCardClicked);
            _newGameButton.onClick.AddListener(OnNewGameClicked);
            _settingsButton?.onClick.AddListener(OnSettingsButtonClicked);
            _drawCardButtonObj.SetActive(false);
            _newGameButtonObj.SetActive(false);
        }

        private void OnDisable()
        {
            _drawCardButton.onClick.RemoveListener(OnDrawCardClicked);
            _newGameButton.onClick.RemoveListener(OnNewGameClicked);
            _settingsButton?.onClick.RemoveListener(OnSettingsButtonClicked);
        }

        private async void OnDrawCardClicked()
        {
            Logger.Log("Draw card clicked.", LogType.Log);
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            if (!await _roundLock.WaitAsync(0))
            {
                Logger.Log("Round already in progress, ignoring extra click.", LogType.Warning);
                return;
            }

            try
            {
                CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();
                if (_gameController != null)
                {
                    _drawCardButtonObj.SetActive(false);
                    await _gameController.PlayRoundAsync(cancellationToken);
                }
                else
                {
                    Logger.Log("Game controller is not set.", LogType.Warning);
                }
            }
            finally
            {
                _drawCardButtonObj.SetActive(true);
                _roundLock.Release();
            }
        }

        private void OnNewGameClicked()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);
            AudioManager.Instance.ResumeMusic();
            ResetGame();
        }

        private void OnSettingsButtonClicked()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);
            Logger.Log("Settings button clicked. Transitioning to Lobby scene...", LogType.Log);
            _ = GameInitializer.Instance.LoadLobbySceneWithLoadingAsync();
        }

        public void Initialize(IGameController gameController)
        {
            _gameController = gameController;
            _drawCardButtonObj.SetActive(true);
        }

        public void WinGame()
        {
            AudioManager.Instance.StopMusic();
            _drawCardButtonObj.SetActive(false);
            _newGameButtonObj.SetActive(true);
            Logger.Log("Game session ended.", LogType.Error);
        }

        public void WinRound()
        {
            Logger.Log("Round ended.", LogType.Log);
        }

        private void ResetGame()
        {
            _gameController?.ResetGame();
            _newGameButtonObj.SetActive(false);
            _drawCardButtonObj.SetActive(true);
        }
    }
}
