using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Views
{
    public interface IBetView
    {
        UniTask InstantiateBetButtons(Vector2[] playersPositions, Action<int> onBetButtonClicked);
        void UpdateScore(int score);
        void ResetView();
    }
}