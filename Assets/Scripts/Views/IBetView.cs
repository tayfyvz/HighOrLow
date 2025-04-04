using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Views
{
    public interface IBetView
    {
        UniTask InstantiateBetButtons(Vector2[] playersPositions, Transform[] playersTransforms, Action<int> onBetButtonClicked);
        void UpdateScore(int score);
        void ResetView();
        UniTask PlayWinBetAnimSeq(int resultComboMultiplier, int resultAwardedPoints, CancellationToken cancellationToken);
        UniTask PlayLoseBetAnimSeq(int resultComboMultiplier, CancellationToken cancellationToken);
        void ResetBet();
        void DeactivateButtonsExcept(int playerIndex);
        void DeactivateButtons();
    }
}