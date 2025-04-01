using Models;
using UnityEngine;
using Utils;
using Views;

namespace Controllers
{
    public class GameController : IGameController
    {
        private readonly IPlayerController _playerController = new PlayerController();
        private readonly IDeckController _deckController = new DeckController();
        private readonly IBetController _betController = new BetController();
        private readonly GameSessionManager _gameSessionManager;
        
        public GameController(GameSessionManager gameSessionManager)
        {
            _gameSessionManager = gameSessionManager;
        }

        /// <summary>
        /// Sets the user's bet based on the provided player index.
        /// </summary>
        public void SetUserBet(int playerIndex)
        {
            _betController.SetBet(playerIndex);
        }
        
        public void PlayRound()
        {
            int playerCount = _playerController.GetPlayerCount();

            // Validate that we have sufficient cards for the players.
            if (!IsDeckSufficient(playerCount))
            {
                HandleInsufficientCards();
                return;
            }

            bool isLastRound = HasEnoughCards(playerCount);

            // Distribute cards to the players.
            Card[] roundCards = DistributeCards(playerCount);

            // Determine the winning card.
            int winningIndex = DetermineWinningIndex(roundCards);

            // Process the round outcome based on the winning index.
            ProcessRoundOutcome(winningIndex);

            if (isLastRound)
            {
                EndGame();
            }
        }
        
        public void ResetGame()
        {
            _deckController.ResetDeck();
            _playerController.ResetPlayers();
            _betController.ResetBet();
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

        public void PassView(IView[] views, Vector2[] positions)
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
                    Player player = new Player($"Player {i + 1}", i);
                    _playerController.AddPlayer(player, playerViews[i]);
                }
                
                _playerController.SetPlayersPosition(positions);
            }
        }

        public void InitializeBetSystem()
        {
            Vector2[] playersPositions = _playerController.GetPlayersPositions();
            _betController.Initialize(playersPositions);
        }

        private bool IsDeckSufficient(int playerCount)
        {
            return _deckController.HasCards(playerCount);
        }
        
        
        private bool HasEnoughCards(int playerCount)
        {
            return _deckController.IsLastRound(playerCount);
        }
        
        private void HandleInsufficientCards()
        {
            Debug.LogError("Not enough cards");
            EndGame();
        }

        private void EndGame()
        {
            IPlayerView sessionWinner = _playerController.MarkSessionWinner();
            _gameSessionManager.WinGame(sessionWinner);
        }

        private Card[] DistributeCards(int playerCount)
        {
            Card[] roundCards = new Card[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                Card card = _deckController.DrawCard();
                roundCards[i] = card;
                _playerController.ReceiveCard(i, card);
            }
            return roundCards;
        }

        private int DetermineWinningIndex(Card[] cards)
        {
            int winningIndex = 0;
            for (int i = 1; i < cards.Length; i++)
            {
                if (CardComparer.Compare(cards[i], cards[winningIndex]) > 0)
                {
                    winningIndex = i;
                }
            }
            return winningIndex;
        }

        private void ProcessRoundOutcome(int winningIndex)
        {
            IPlayerView roundWinner = _playerController.MarkRoundWinner(winningIndex);
            _gameSessionManager.WinRound(roundWinner);
            _betController.EvaluateBet(winningIndex, 10);
        }
    }
}