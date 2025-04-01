using UnityEngine;
using Views;

namespace Controllers
{
    public interface IGameController
    {
        void SetUserBet(int playerIndex);
        void PlayRound();
        void ResetGame();
        void PassView(IView view);
        void PassView(IView[] views, Vector2[] positions);
        void InitializeBetSystem();
    }
}