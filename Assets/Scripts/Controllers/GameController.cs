using Models;
using Utils;
using Views;

namespace Controllers
{
    public class GameController
    {
        public DeckController DeckCtrl { get; private set; }
        public PlayerController[] PlayerControllers { get; private set; }
        public BetController BetCtrl { get; private set; }

        /// <summary>
        /// Constructs the game controller.
        /// The number of players is determined by the length of the playerViews array.
        /// Each player gets one PlayerModel and one PlayerController.
        /// </summary>
        public GameController(PlayerView[] playerViews)
        {
            int playerCount = playerViews.Length;
            DeckCtrl = new DeckController();
            PlayerControllers = new PlayerController[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                // Create each player's model and controller.
                Player player = new Player($"Player {i + 1}");
                PlayerController controller = new PlayerController(player);
                // Register the corresponding PlayerView with the PlayerController.
                controller.RegisterView(playerViews[i]);
                PlayerControllers[i] = controller;
            }

            BetCtrl = new BetController();
        }

        /// <summary>
        /// Sets the user's bet.
        /// </summary>
        public void SetUserBet(int playerIndex)
        {
            BetCtrl.SetBet(playerIndex);
        }

        /// <summary>
        /// Plays one round:
        /// 1. Draws one card per player.
        /// 2. Determines the winning card (by rank then suit).
        /// 3. Uses BetController to evaluate the bet.
        /// </summary>
        public void PlayRound()
        {
            int n = PlayerControllers.Length;
            Card[] roundCards = new Card[n];

            for (int i = 0; i < n; i++)
            {
                if (!DeckCtrl.HasCards(1))
                    DeckCtrl.ResetDeck();
                Card card = DeckCtrl.DrawCard();
                roundCards[i] = card;
                PlayerControllers[i].ReceiveCard(card);
            }

            // Determine the winner by comparing cards.
            int winningIndex = 0;
            for (int i = 1; i < n; i++)
            {
                if (CardComparer.Compare(roundCards[i], roundCards[winningIndex]) > 0)
                    winningIndex = i;
            }

            BetCtrl.EvaluateBet(winningIndex, 10);
        }
    }
}