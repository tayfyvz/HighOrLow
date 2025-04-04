using System.Threading;
using Cysharp.Threading.Tasks;
using Models;
using UnityEngine;
using Utils;
using Views;
using Logger = Utils.Logger;

namespace Controllers
{
    public class GameController : IGameController
    {
        private readonly IPlayerController _playerController = new PlayerController();
        private readonly IDeckController _deckController = new DeckController();
        private readonly IBetController _betController = new BetController();
        private readonly IAnimationController _animationController;
        private readonly GameSessionManager _gameSessionManager;

        public GameController(GameSessionManager gameSessionManager, IAnimationController animationController)
        {
            _gameSessionManager = gameSessionManager;
            _animationController = animationController;
        }

        public async UniTask PlayRoundAsync(CancellationToken cancellationToken)
        {
            int playerCount = _playerController.GetPlayerCount();

            if (!IsDeckSufficient(playerCount))
            {
                HandleInsufficientCards();
                return;
            }
            
            _betController.DeactivateButtons();

            bool isLastRound = HasEnoughCards(playerCount);
            Card[] roundCards = DistributeCards(playerCount);
            await _animationController.AnimateCardDistributionAsync(roundCards, cancellationToken);

            int winningIndex = DetermineWinningIndex(roundCards);
            _playerController.MarkRoundWinner(winningIndex);
            _gameSessionManager.WinRound();
            await _animationController.AnimateRoundWinningAsync(winningIndex, cancellationToken);
            _playerController.SetPlayerScore(winningIndex);

            BetResult betResult = _betController.EvaluateBet(winningIndex, 10);
            await _animationController.AnimateBetResultAsync(betResult, winningIndex, cancellationToken);
            _betController.UpdateScore();

            if (isLastRound)
            {
                int winnerIndex = EndGame();
                await _animationController.AnimateSessionWinningAsync(winnerIndex, cancellationToken);
            }

            Logger.Log("Ready for new round.", LogType.Log);
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
                    break;
                case IBetView betView:
                    _betController.AttachView(betView);
                    break;
            }
        }

        public void PassView(IView[] views, Vector2[] positions)
        {
            if (views.Length == 0) return;

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
            Transform[] playersTransforms = _playerController.GetPlayersTransforms();
            _betController.Initialize(playersPositions, playersTransforms);
        }

        private bool IsDeckSufficient(int playerCount) => _deckController.HasCards(playerCount);

        private bool HasEnoughCards(int playerCount) => _deckController.IsLastRound(playerCount);

        private void HandleInsufficientCards()
        {
            Logger.Log("Not enough cards.", LogType.Error);
            EndGame();
        }

        private int EndGame()
        {
            _gameSessionManager.WinGame();
            return _playerController.MarkSessionWinner();
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
                if (CardComparer.Compare(cards[i], cards[winningIndex]) > 0) winningIndex = i;
            }
            return winningIndex;
        }
    }
}
