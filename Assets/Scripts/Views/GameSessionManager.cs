using Controllers;
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
        
        private void OnDrawCardClicked()
        {
            _gameController?.PlayRound();
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
        
        public void WinGame(IPlayerView winner)
        {
            _drawCardButtonObj.SetActive(false);
            
            Debug.LogError("WIN session");
            
            // anim or something
            // wait..
            
            _newGameButtonObj.SetActive(true);
        }

        public void WinRound(IPlayerView roundWinner)
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