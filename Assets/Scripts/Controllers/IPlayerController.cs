using Models;
using UnityEngine;
using Views;

namespace Controllers
{
    public interface IPlayerController
    {
        void AddPlayer(Player player, IPlayerView view);
        void SetPlayersPosition(Vector2[] positions);
        void ReceiveCard(int playerId, Card card);
        void MarkRoundWinner(int winningIndex);
        void MarkSessionWinner();
        void ResetPlayers();
        int GetPlayerCount();
    }
}