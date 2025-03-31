using UnityEngine;
using Views;

namespace Controllers
{
    public class BetController : BaseController<IBetView>, IBetController
    {
        // -1 indicates no bet is active.
        public int CurrentBetIndex { get; private set; } = -1;
        public int UserScore { get; private set; } = 0;

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
            System.Console.WriteLine($"User bet set to Player {playerIndex + 1}");
            UpdateView();
        }

        /// <summary>
        /// Evaluates the bet. Awards the specified points if the bet is correct.
        /// After evaluation, the bet is reset.
        /// </summary>
        public bool EvaluateBet(int winningPlayerIndex, int pointsAwarded)
        {
            bool correct = CurrentBetIndex == winningPlayerIndex;
            if (correct)
            {
                UserScore += pointsAwarded;
                Debug.LogError("Bet correct. Score increased by " + pointsAwarded);
            }
            else
            {
                Debug.LogError("Bet incorrect.");
            }

            CurrentBetIndex = -1;
            UpdateView();
            return correct;
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