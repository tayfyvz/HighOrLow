using UnityEngine;
using Views;

namespace Controllers
{
    public struct BetResult
    {
        public bool IsCorrect { get; set; }
        public int AwardedPoints { get; set; }
        public int ComboMultiplier { get; set; }
    }

    public class BetController : BaseController<IBetView>, IBetController
    {
        // -1 indicates no bet is active.
        public int CurrentBetIndex { get; private set; } = -1;
        public int UserScore { get; private set; } = 0;

        private int _comboMultiplier = 1;

        public void Initialize(Vector2[] playersPositions)
        {
            View.InstantiateBetButtons(playersPositions, SetBet);
        }

        /// <summary>
        /// Attaches the BetView so that this controller updates it.
        /// </summary>
        public void SetBet(int playerIndex)
        {
            if (playerIndex < 0)
            {
                return;
            }

            CurrentBetIndex = playerIndex;
            Debug.Log($"User bet set to Player {playerIndex + 1}");
            UpdateView();
        }

        public void ResetBet()
        {
            CurrentBetIndex = -1;
            UserScore = 0;
            _comboMultiplier = 1;
            if (View != null)
            {
                View.ResetView();
            }
            else
            {
                Debug.LogError("Bet controller view is null");
            }
        }

        /// <summary>
        /// Evaluates the bet. Awards the specified points if the bet is correct.
        /// After evaluation, the bet is reset.
        /// </summary>
        // public bool EvaluateBet(int winningPlayerIndex, int basePoints)
        // {
        //     bool correct = CurrentBetIndex == winningPlayerIndex;
        //     if (correct)
        //     {
        //         int awardedPoints = basePoints * _comboMultiplier;
        //         UserScore += awardedPoints;
        //         Debug.Log($"Bet correct. Awarded {basePoints} x {_comboMultiplier} = {awardedPoints} points.");
        //         _comboMultiplier++; // Increment combo for subsequent correct bets.
        //     }
        //     else
        //     {
        //         Debug.Log("Bet incorrect. Combo reset.");
        //         _comboMultiplier = 1;
        //     }
        //
        //     // Reset the active bet.
        //     CurrentBetIndex = -1;
        //     UpdateView();
        //     return correct;
        // }
        
        public BetResult EvaluateBet(int winningPlayerIndex, int basePoints)
        {
            return CurrentBetIndex == winningPlayerIndex ? EvaluateCorrectBet(basePoints) : EvaluateIncorrectBet();
        }
        
        private BetResult EvaluateCorrectBet(int basePoints)
        {
            BetResult result = new BetResult
            {
                IsCorrect = true,
                ComboMultiplier = _comboMultiplier,
                AwardedPoints = basePoints * _comboMultiplier,
            };

            UserScore += result.AwardedPoints;
            Debug.Log($"Bet correct. Awarded {basePoints} x {_comboMultiplier} = {result.AwardedPoints} points.");
            _comboMultiplier++; // Increment multiplier for subsequent correct bets.
            CurrentBetIndex = -1;
            UpdateView();
            return result;
        }

        private BetResult EvaluateIncorrectBet()
        {
            BetResult result = new BetResult
            {
                IsCorrect = false,
                ComboMultiplier = 1,
                AwardedPoints = 0
            };

            Debug.Log("Bet incorrect. Combo reset.");
            _comboMultiplier = 1;
            CurrentBetIndex = -1;
            UpdateView();
            return result;
        }


        protected override void UpdateView()
        {
            if (View != null)
            {
                View.UpdateScore(UserScore);
                return;
            }

            Debug.LogError("bet controller view null");
        }
    }
}