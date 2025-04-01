using Models;
using UnityEngine;

namespace Views
{
    public interface IPlayerView
    {
        void SetPlayerName(string name);
        void UpdateHand(Card card);
        void SetPosition(Vector2 position);
        void SetScore(int score);
        void ResetView();
        Vector2 GetPosition();
    }
}