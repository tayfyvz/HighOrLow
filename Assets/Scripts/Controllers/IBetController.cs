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
        BetResult EvaluateBet(int winningPlayerIndex, int basePoints);
    }
}