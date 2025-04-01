using UnityEngine;
using Views;

namespace Controllers
{
    public interface IBetController
    {
        void Initialize(Vector2[] playersPositions);
        void AttachView(IBetView view);
        void SetBet(int playerIndex);
        void ResetBet();
        bool EvaluateBet(int winningPlayerIndex, int pointsAwarded);
    }
}