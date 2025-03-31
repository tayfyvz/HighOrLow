using Views;

namespace Controllers
{
    public interface IGameController
    {
        void SetUserBet(int playerIndex);
        void PlayRound();
        void PassView(IView view);
        void PassView(IView[] views);
    }
}