using UnityEngine;
using Views;
using Logger = Utils.Logger;

namespace Controllers
{
    public struct BetResult
    {
        public bool IsBet { get; set; }
        public int AwardedPoints { get; set; }
        public int ComboMultiplier { get; set; }
    }

    public class BetController : BaseController<IBetView>, IBetController
    {
        // -1 indicates no bet is active.
        public int CurrentBetIndex { get; private set; } = -1;
        public int UserScore { get; private set; } = 0;

        private int _comboMultiplier = 1;

        public void Initialize(Vector2[] playersPositions, Transform[] playersTransforms)
        {
            View.InstantiateBetButtons(playersPositions, playersTransforms, SetBet);
        }

        public void SetBet(int playerIndex)
        {
            if (playerIndex < 0)
            {
                return;
            }

            CurrentBetIndex = playerIndex;
            Logger.Log($"User bet set to Player {playerIndex + 1}");
            View.DeactivateButtonsExcept(playerIndex);
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
                View.ResetBet();
            }
            else
            {
                Debug.LogError("Bet controller view is null");
            }
        }

        public BetResult EvaluateBet(int winningPlayerIndex, int basePoints)
        {
            return CurrentBetIndex == winningPlayerIndex ? EvaluateCorrectBet(basePoints) : EvaluateIncorrectBet();
        }

        public void UpdateScore()
        {
            UpdateView();
        }

        private BetResult EvaluateCorrectBet(int basePoints)
        {
            BetResult result = new BetResult
            {
                IsBet = true,
                ComboMultiplier = _comboMultiplier,
                AwardedPoints = basePoints * _comboMultiplier,
            };

            UserScore += result.AwardedPoints;
            Logger.Log($"Bet correct. Awarded {basePoints} x {_comboMultiplier} = {result.AwardedPoints} points.");
            _comboMultiplier++;
            CurrentBetIndex = -1;
            return result;
        }

        private BetResult EvaluateIncorrectBet()
        {
            BetResult result = new BetResult
            {
                IsBet = false,
                ComboMultiplier = 1,
                AwardedPoints = 0
            };

            if (CurrentBetIndex == -1)
            {
                result.IsBet = false;
            }

            Debug.Log("Bet incorrect. Combo reset.");
            _comboMultiplier = 1;
            CurrentBetIndex = -1;
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