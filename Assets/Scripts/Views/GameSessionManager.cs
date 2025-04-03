using Controllers;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class GameSessionManager : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button _drawCardButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _settingsButton; // New Settings Button

        [Header("GameObject References")]
        [SerializeField] private GameObject _drawCardButtonObj;
        [SerializeField] private GameObject _newGameButtonObj;

        private IGameController _gameController;

        private void Start()
        {
            // Initialize button objects if not assigned directly.
            if (_drawCardButtonObj == null && _drawCardButton != null)
            {
                _drawCardButtonObj = _drawCardButton.gameObject;
            }

            if (_newGameButtonObj == null && _newGameButton != null)
            {
                _newGameButtonObj = _newGameButton.gameObject;
            }

            // Ensure the settings button is active.
            if (_settingsButton != null)
            {
                _settingsButton.gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            // Add listeners for button actions.
            _drawCardButton.onClick.AddListener(OnDrawCardClicked);
            _newGameButton.onClick.AddListener(OnNewGameClicked);
            _settingsButton?.onClick.AddListener(OnSettingsButtonClicked); // Listener for Settings Button

            // Initially disable draw card and new game buttons.
            _drawCardButtonObj.SetActive(false);
            _newGameButtonObj.SetActive(false);
        }

        private void OnDisable()
        {
            // Remove listeners to avoid memory leaks.
            _drawCardButton.onClick.RemoveListener(OnDrawCardClicked);
            _newGameButton.onClick.RemoveListener(OnNewGameClicked);
            _settingsButton?.onClick.RemoveListener(OnSettingsButtonClicked);
        }

        private async void OnDrawCardClicked()
        {
            // Handle logic for drawing a card during the game.
            Debug.Log("Draw card clicked.");

            if (_gameController != null)
            {
                await _gameController.PlayRoundAsync(this.GetCancellationTokenOnDestroy());
            }
            else
            {
                Debug.LogWarning("Game controller is not set.");
            }
        }

        private void OnNewGameClicked()
        {
            // Reset the game to its initial state.
            Debug.Log("New game clicked.");
            ResetGame();
        }

        private void OnSettingsButtonClicked()
        {
            // Transition from Game to Lobby scene when settings are clicked.
            Debug.Log("Settings button clicked. Transitioning to Lobby scene...");
            GameInitializer.Instance.LoadLobbySceneWithLoadingAsync();
        }

        public void Initialize(IGameController gameController)
        {
            // Set up the game controller and activate relevant buttons.
            _gameController = gameController;

            _drawCardButtonObj.SetActive(true);
        }

        public void WinGame()
        {
            // Handle end-of-game logic.
            _drawCardButtonObj.SetActive(false);
            _newGameButtonObj.SetActive(true);

            Debug.LogError("Game session ended.");
        }

        public void WinRound()
        {
            // Handle logic for the end of a round.
            _drawCardButtonObj.SetActive(false);

            Debug.Log("Round ended.");

            _drawCardButtonObj.SetActive(true);
        }

        private void ResetGame()
        {
            // Reset the game and ensure UI state is restored.
            _gameController?.ResetGame();
            _newGameButtonObj.SetActive(false);
            _drawCardButtonObj.SetActive(true);
        }
    }
}
