using System.Collections.Generic;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace Views
{
    public class GameView : MonoBehaviour
    {
        [Header("Prefabs & Containers")]
        public GameObject PlayerViewPrefab; // Prefab for PlayerView (assigned via Inspector)

        public Transform PlayerViewsContainer; // Parent container for dynamically instantiated PlayerViews
        public GameObject BetButtonPrefab; // Prefab for a bet button (contains a Button component & a Text)
        public Transform BetButtonsContainer; // Parent container for bet button instances

        [Header("Other UI Elements")] public DeckView DeckView; // DeckView assigned via Inspector
        public BetView BetView; // BetView assigned via Inspector
        public Button PlayRoundButton; // Button to play a round (assigned via Inspector)

        private GameController _gameController;

        void Start()
        {
            // Get settings from an external class (set on the Main scene)
            int playerCount = SettingsController.Instance.GetPlayerCount();

            // Dynamically create an array of PlayerView instances based on player count.
            PlayerView[] playerViews = new PlayerView[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                GameObject pvGO = Instantiate(PlayerViewPrefab, PlayerViewsContainer);
                PlayerView pv = pvGO.GetComponent<PlayerView>();
                playerViews[i] = pv;
            }

            // Create the GameController using player views.
            
            _gameController = new GameController(playerViews);

            // Attach the DeckView and BetView to their corresponding controllers.
            _gameController.DeckCtrl.AttachView(DeckView);
            _gameController.BetCtrl.AttachView(BetView);

            // Set up the Play Round button to listen for click events.
            if (PlayRoundButton != null)
            {
                PlayRoundButton.onClick.AddListener(OnPlayRoundButtonClick);
            }

            // Dynamically create bet buttons based on the player count.
            if (BetButtonsContainer != null && BetButtonPrefab != null)
            {
                for (int i = 0; i < playerCount; i++)
                {
                    GameObject betBtnObj = Instantiate(BetButtonPrefab, BetButtonsContainer);
                    Button betBtn = betBtnObj.GetComponent<Button>();
                    int index = i; // Capture the current player index for the closure.
                    betBtn.onClick.AddListener(() => OnBetButtonClick(index));

                    // Optionally, update the button text.
                    Text btnText = betBtnObj.GetComponentInChildren<Text>();
                    if (btnText != null)
                        btnText.text = "Bet on Player " + (i + 1);
                }
            }
        }

        /// <summary>
        /// Called when a bet button is clicked.
        /// Sets the user's bet to the corresponding player.
        /// </summary>
        /// <param name="playerIndex">Index of the player on whom the user bets.</param>
        public void OnBetButtonClick(int playerIndex)
        {
            _gameController.SetUserBet(playerIndex);
        }

        /// <summary>
        /// Called when the Play Round button is clicked.
        /// </summary>
        public void OnPlayRoundButtonClick()
        {
            _gameController.PlayRound();
        }
    }
}