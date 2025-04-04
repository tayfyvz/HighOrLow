using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Models;
using UnityEngine;
using Views;
using Logger = Utils.Logger;

namespace Controllers
{
    public class AnimationController : IAnimationController
    {
        private readonly IPlayerView[] _players;
        private readonly IDeckView _deckView;
        private readonly IBetView _betView;

        public AnimationController(IPlayerView[] players, IDeckView deckView, IBetView betView)
        {
            _players = players;
            _deckView = deckView;
            _betView = betView;
        }

        public async UniTask AnimateCardDistributionAsync(Card[] roundCards, CancellationToken cancellationToken)
        {
            int count = roundCards.Length;
            _deckView.PlayDrawCardAmountAnim(count);

            for (int i = 0; i < count; i++)
            {
                Logger.Log($"Starting animation for card {roundCards[i]} for player {i}.", LogType.Log);
                Vector2 destination = _players[i].GetCardPosition();
                await _deckView.PlayDistributeCardAnim(roundCards[i], destination, cancellationToken);
                Logger.Log($"Completed animation for card {roundCards[i]} for player {i}.", LogType.Log);
            }

            foreach (IPlayerView player in _players)
            {
                player.PlayReadCardAnim();
            }

            await _deckView.PlayFlipCardAnim();
            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: cancellationToken);
        }

        public async UniTask AnimateRoundWinningAsync(int winningIndex, CancellationToken cancellationToken)
        {
            Logger.Log("Starting animation for round winning.", LogType.Log);
            await _deckView.PlayDiscardCardsAnim();
            await _players[winningIndex].PlayWinScoreAnim();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
            Logger.Log("Completed animation for round winning.", LogType.Log);
        }

        public async UniTask AnimateBetResultAsync(BetResult result, int winningPlayerIndex, CancellationToken cancellationToken)
        {
            if (result.IsBet)
            {
                Logger.Log($"Animating CORRECT bet: Combo {result.ComboMultiplier}, Awarded {result.AwardedPoints} points for player {winningPlayerIndex}.", LogType.Log);
                await _betView.PlayWinBetAnimSeq(result.ComboMultiplier, result.AwardedPoints, cancellationToken);
            }
            else
            {
                Logger.Log($"Animating INCORRECT bet for player {winningPlayerIndex}.", LogType.Log);
                await _betView.PlayLoseBetAnimSeq(result.ComboMultiplier, cancellationToken);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
            _betView.ResetBet();
            Logger.Log("Completed animation for BET.", LogType.Log);
        }

        public async UniTask AnimateSessionWinningAsync(int winnerIndex, CancellationToken cancellationToken)
        {
            Logger.Log("Started animation for SessionWin.", LogType.Log);
            await _players[winnerIndex].PlayWinSessionAnim();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
            Logger.Log("Completed animation for SessionWin.", LogType.Log);
        }
    }
}
