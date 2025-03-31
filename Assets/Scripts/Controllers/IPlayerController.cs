using Models;
using Views;

namespace Controllers
{
    public interface IPlayerController
    {
        void AddPlayer(Player player, IPlayerView view);
        void RemoveView(IPlayerView view);
        void ReceiveCard(int playerId, Card card);
        int GetPlayerCount();
    }
}