using Models;
using Views;

namespace Controllers
{
    public interface IDeckController
    {
        void AttachView(IDeckView view);
        void ResetDeck();
        bool HasCards(int count);
        bool IsLastRound(int playerCount);
        Card DrawCard();
    }
}