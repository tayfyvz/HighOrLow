using Cysharp.Threading.Tasks;
using Models;
using UnityEngine;

namespace Views
{
    public interface IPlayerView
    {
        void SetPlayerName(string name);
        void UpdateHand(Card card, int id);
        void SetPosition(Vector2 position);
        void SetScore(int score);
        void ResetView();
        Vector2 GetButtonPosition();
        Vector2 GetCardPosition();
        void PlayReadCardAnim();
        UniTask PlayWinScoreAnim();
        UniTask PlayWinSessionAnim();
        Transform GetTransform();
    }
}