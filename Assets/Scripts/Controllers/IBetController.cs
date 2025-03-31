using Views;

namespace Controllers
{
    public interface IBetController
    {
        void AttachView(IBetView view);
        void SetBet(int playerIndex);
        bool EvaluateBet(int winningPlayerIndex, int pointsAwarded);
    }
}