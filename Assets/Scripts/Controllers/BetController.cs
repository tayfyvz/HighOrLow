using Views;

namespace Controllers
{
    public class BetController
    {
        // -1 indicates no bet is active.
        public int CurrentBetIndex { get; private set; } = -1;
        public int UserScore { get; private set; } = 0;

        private BetView betView; // Reference to the UI view for bet info

        /// <summary>
        /// Attaches the BetView so that this controller updates it.
        /// </summary>
        public void AttachView(BetView view)
        {
            betView = view;
            UpdateView();
        }

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
            bool correct = (CurrentBetIndex == winningPlayerIndex);
            if (correct)
            {
                UserScore += pointsAwarded;
                System.Console.WriteLine("Bet correct. Score increased by " + pointsAwarded);
            }
            else
            {
                System.Console.WriteLine("Bet incorrect.");
            }

            CurrentBetIndex = -1;
            UpdateView();
            return correct;
        }

        private void UpdateView()
        {
            if (betView != null)
                betView.UpdateScore(UserScore);
        }
    }
}