using Models;

namespace Controllers
{
    public interface IDeckController
    {
        void ResetDeck();
        bool HasCards(int count);
        Card DrawCard();
    }
}