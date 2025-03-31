using Models;
using UnityEngine;
using Utils;
using Views;

namespace Controllers
{
    public class GameController : IGameController
    {
        private readonly PlayerController _playerController;
        private readonly DeckController _deckController;
        private readonly BetController _betController;

        public GameController()
        {
            _deckController = new DeckController();
            _playerController = new PlayerController();
            _betController = new BetController();
        }

        /// <summary>
        /// Sets the user's bet based on the provided player index.
        /// </summary>
        public void SetUserBet(int playerIndex)
        {
            _betController.SetBet(playerIndex);
        }

        /// <summary>
        /// Plays one round:
        /// 1. Draws one card per player.
        /// 2. Determines the winning card (by rank then suit).
        /// 3. Uses BetController to evaluate the bet.
        /// </summary>
        public void PlayRound()
        {
            int n = _playerController.PlayerCount;
            Card[] roundCards = new Card[n];

            for (int i = 0; i < n; i++)
            {
                if (!_deckController.HasCards(1))
                    _deckController.ResetDeck();
                Card card = _deckController.DrawCard();
                roundCards[i] = card;
                // Direct the centralized PlayerController to update the appropriate player.
                _playerController.ReceiveCard(i, card);
            }

            // Determine the winner by comparing cards.
            int winningIndex = 0;
            for (int i = 1; i < n; i++)
            {
                if (CardComparer.Compare(roundCards[i], roundCards[winningIndex]) > 0)
                    winningIndex = i;
            }

            _betController.EvaluateBet(winningIndex, 10);
        }

        public void PassView(IView view)
        {
            switch (view)
            {
                case IDeckView deckView:
                    _deckController.AttachView(deckView);
                    return;
                case IBetView betView:
                    _betController.AttachView(betView);
                    return;
            }
        }

        public void PassView(IView[] views)
        {
            if (views.Length == 0)
            {
                return;
            }

            if (views is IPlayerView[] playerViews)
            {
                int playerCount = playerViews.Length;

                for (int i = 0; i < playerCount; i++)
                {
                    // Create each player's model, providing a unique ID for each.
                    // (Assuming Player class constructor overload: Player(string name, int id))
                    Player player = new Player($"Player {i + 1}", i);
                    // Add the player and register the corresponding view with the centralized PlayerController.
                    _playerController.AddPlayer(player, playerViews[i]);
                }
            }
        }
    }
}