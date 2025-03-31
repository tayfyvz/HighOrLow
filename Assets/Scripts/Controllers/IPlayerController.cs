using Models;
using Views;

namespace Controllers
{
    public interface IPlayerController
    {
        void RemoveView(IPlayerView view);
        void ReceiveCard(Card card);
    }
}