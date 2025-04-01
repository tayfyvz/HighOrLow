using System;
using UnityEngine;

namespace Views
{
    public interface IBetView
    {
        void InstantiateBetButtons(Vector2[] playersPositions, Action<int> onBetButtonClicked);
        void UpdateScore(int score);
        void ResetView();
    }
}