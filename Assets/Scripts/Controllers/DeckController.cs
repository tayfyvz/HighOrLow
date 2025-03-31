using Models;
using UnityEngine;
using Utils;
using Views;

namespace Controllers
{
    public class DeckController : BaseController<IDeckView>, IDeckController
    {
        public Deck DeckModel { get; private set; }

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
            if (View != null)
            {
                View.UpdateDeckCount(DeckModel.RemainingCardsCount);
                return;
            }
            
            Debug.LogError("deck controller view null");
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