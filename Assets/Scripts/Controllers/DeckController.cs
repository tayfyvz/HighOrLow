using Models;
using Utils;
using Views;

namespace Controllers
{
    public class DeckController : BaseController<IDeckView>, IDeckController
    {
        public Deck DeckModel { get; private set; }
        private DeckView deckView; // Reference to the UI view for deck info

        public DeckController()
        {
            IRandomNumberGenerator rng = new SystemRandomNumberGenerator();
            DeckModel = new Deck(rng);
        }

        /// <summary>
        /// Attaches the DeckView and updates it.
        /// </summary>
        /*public void AttachView(DeckView view)
        {
            deckView = view;
            UpdateView();
        }*/

        /// <summary>
        /// Call this method after any operation that might have changed the deck.
        /// </summary>
        protected override void UpdateView()
        {
            if (deckView != null)
            {
                deckView.UpdateDeckCount(DeckModel.RemainingCardsCount);
            }
        }

        public Card DrawCard()
        {
            Card drawn = DeckModel.DrawCard();
            UpdateView();
            return drawn;
        }

        public bool HasCards(int count)
        {
            return DeckModel.HasEnoughCards(count);
        }

        public void ResetDeck()
        {
            DeckModel.Reset();
            UpdateView();
        }
    }
}