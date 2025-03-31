namespace Controllers
{
    public interface IBetController
    {
        void SetBet(int playerIndex);
        bool EvaluateBet(int winningPlayerIndex, int pointsAwarded);
    }
}