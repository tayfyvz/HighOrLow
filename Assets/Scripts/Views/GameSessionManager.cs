using System.Threading;
using Controllers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class GameSessionManager : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button _drawCardButton;
        [SerializeField] private Button _newGameButton;
        
        [Header("GameObject References")]
        [SerializeField] private GameObject _drawCardButtonObj;
        [SerializeField] private GameObject _newGameButtonObj;
        
        private IGameController _gameController;

        private readonly SemaphoreSlim _roundLock = new SemaphoreSlim(1, 1);
        
        private void Start()
        {
            if (_drawCardButtonObj == null && _drawCardButton != null)
            {
                _drawCardButtonObj = _drawCardButton.gameObject;
            }

            if (_newGameButtonObj == null && _newGameButton != null)
            {
                _newGameButtonObj = _newGameButton.gameObject;
            }
        }

        private void OnEnable()
        {
            _drawCardButton.onClick.AddListener(OnDrawCardClicked);
            _newGameButton.onClick.AddListener(OnNewGameClicked);
            
            _drawCardButtonObj.SetActive(false);
            _newGameButtonObj.SetActive(false);
        }
        
        private void OnDisable()
        {
            _drawCardButton.onClick.RemoveListener(OnDrawCardClicked);
            _newGameButton.onClick.RemoveListener(OnNewGameClicked);
        }
        
        private async void OnDrawCardClicked()
        {
            if (!await _roundLock.WaitAsync(0))
            {
                Debug.Log("Round already in progress, ignoring extra click.");
                return;
            }

            try
            {
                // Retrieve a cancellation token from this MonoBehaviour.
                CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();
                
                if (_gameController != null)
                {
                    await _gameController.PlayRoundAsync(cancellationToken);
                }
                else
                {
                    Debug.LogWarning("Game controller is not set.");
                }
            }
            finally
            {
                // Always ensure the lock is released.
                _roundLock.Release();
            }
        }
        
        private void OnNewGameClicked()
        {
            ResetGame();
        }

        public void Initialize(IGameController gameController)
        {
            _gameController = gameController;
            
            _drawCardButtonObj.SetActive(true);
        }
        
        public void WinGame()
        {
            _drawCardButtonObj.SetActive(false);
            
            Debug.LogError("WIN session");
            
            // anim or something
            // wait..
            
            _newGameButtonObj.SetActive(true);
        }

        public void WinRound()
        {
            _drawCardButtonObj.SetActive(false);

            Debug.LogError("WINRound");
            
            // anim or something
            // wait..
            
            _drawCardButtonObj.SetActive(true);
        }
        
        
        private void ResetGame()
        {
            _gameController?.ResetGame();
            _newGameButtonObj.SetActive(false);
            _drawCardButtonObj.SetActive(true);
        }
    }
}