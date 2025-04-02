using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Models;
using UnityEngine;
using Views;

namespace Controllers
{
    public class AnimationController : IAnimationController
    {
        IPlayerView[] _players;
        IDeckView _deckView;
        IBetView _betView;

        public AnimationController(IPlayerView[] players, IDeckView deckView, IBetView betView)
        {
            _players = players;
            _deckView = deckView;
            _betView = betView;
        }

        public async UniTask AnimateCardDistributionAsync(Card[] roundCards, CancellationToken cancellationToken)
        {
            for (int i = 0; i < roundCards.Length; i++)
            {
                // Log start animation (or initiate actual animation logic)
                Debug.Log($"Starting animation for card {roundCards[i].ToString()} for player {i}.");

                // Simulate an animation delay. Replace this with your actual animation code.
                await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cancellationToken);

                // Log completion of the animation for this card.
                Debug.Log($"Completed animation for card {roundCards[i].ToString()} for player {i}.");
            }
        }

        public async UniTask AnimateRoundWinningAsync(int winningIndex, CancellationToken cancellationToken)
        {
            Debug.Log($"Starting animation for round winning.");
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cancellationToken);
            Debug.Log($"Completed animation for round winning.");
        }
        
        public async UniTask AnimateBetResultAsync(BetResult result, int winningPlayerIndex, CancellationToken cancellationToken)
        {
            if (result.IsCorrect)
            {
                Debug.Log($"Animating CORRECT bet: Combo {result.ComboMultiplier}, Awarded {result.AwardedPoints} points for player {winningPlayerIndex}");
                // Insert your animation logic for a correct bet here.
                await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cancellationToken);
            }
            else
            {
                Debug.Log($"Animating INCORRECT bet for player {winningPlayerIndex}");
                // Insert your animation logic for an incorrect bet here.
                await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cancellationToken);
            }
            
            Debug.Log($"Completed animation for BET.");

        }

        public async UniTask AnimateSessionWinningAsync(int winnerIndex, CancellationToken cancellationToken)
        {
            Debug.Log($"Started animation for SessionWin.");

            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cancellationToken);
            Debug.Log($"Completed animation for SessionWin.");

        }
    }
}