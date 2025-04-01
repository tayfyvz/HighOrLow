using System;
using Controllers;
using UnityEngine;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;

namespace Views
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("View References")]
        [SerializeField] private PlayerView _playerViewPrefab;
        [SerializeField] private DeckView _deckView;
        [SerializeField] private BetView _betView;

        [Header("Player References")]
        [SerializeField] private Transform _playerViewsContainer;
        [SerializeField] private Transform[] _playerSitPoints;
        
        [SerializeField] private Button _playRoundButton;

        private IGameController _gameController;

        private void Start()
        {
            int playerCount = SettingsController.Instance.GetPlayerCount();

            PlayerView[] playerViews = new PlayerView[playerCount];
            
            for (int i = 0; i < playerCount; i++)
            {
                playerViews[i] = Instantiate(_playerViewPrefab, _playerViewsContainer);
            }
            
            Vector2[] playerSitPoints = new Vector2[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                playerSitPoints[i] = _playerSitPoints[i].position;
            }

            _gameController = new GameController();
            
            _gameController.PassView(playerViews, playerSitPoints);
            _gameController.PassView(_deckView);
            _gameController.PassView(_betView);
            
            _playRoundButton.onClick.AddListener(OnPlayRoundButtonClicked);

            /*// Set up the Play Round button.
            playRoundButton.onClick.AddListener(() => gameCtrl.PlayRound());
*/
            // Dynamically create bet buttons corresponding to each player.
            /*for (int i = 0; i < playerCount; i++)
            {
                GameObject betBtnObj = Instantiate(_betButton, playerViews[i].transform.position, Quaternion.identity, playerViews[i].transform);
                Button betBtn = betBtnObj.GetComponent<Button>();
                int index = i;
                betBtn.onClick.AddListener(() => _gameController.SetUserBet(index));

                Text btnText = betBtnObj.GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.text = "Bet on Player " + (i + 1);
                }
            }*/
        }

        private void OnPlayRoundButtonClicked()
        {
            _gameController.PlayRound();
        }
    }
}