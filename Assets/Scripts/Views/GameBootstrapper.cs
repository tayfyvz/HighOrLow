using Controllers;
using UnityEngine;

namespace Views
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("View References")]
        [SerializeField] private PlayerView _playerViewPrefab;
        [SerializeField] private DeckView _deckView;
        [SerializeField] private BetView _betView;

        [SerializeField] private Transform _playerViewsContainer;

        private void Start()
        {
            int playerCount = SettingsController.Instance.GetPlayerCount();

            IView[] playerViews = new IView[playerCount];
            
            for (int i = 0; i < playerCount; i++)
            {
                playerViews[i] = Instantiate(_playerViewPrefab, _playerViewsContainer);
            }

            GameController gameController = new GameController();
            
            gameController.PassView(playerViews);
            gameController.PassView(_deckView);
            gameController.PassView(_betView);

            /*// Set up the Play Round button.
            playRoundButton.onClick.AddListener(() => gameCtrl.PlayRound());

            // Dynamically create bet buttons corresponding to each player.
            for (int i = 0; i < playerCount; i++)
            {
                GameObject betBtnObj = Instantiate(betButtonPrefab, betButtonsContainer);
                Button betBtn = betBtnObj.GetComponent<Button>();
                int index = i;
                betBtn.onClick.AddListener(() => gameCtrl.SetUserBet(index));

                Text btnText = betBtnObj.GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.text = "Bet on Player " + (i + 1);
                }
            }*/
        }
    }
}