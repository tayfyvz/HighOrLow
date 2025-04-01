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

        protected override void UpdateView()
        {
            if (View != null)
            {
                View.UpdateDeckCount(DeckModel.RemainingCardsCount);
                return;
            }
            
            Debug.LogError("deck controller view null");
        }

        public bool IsLastRound(int playerCount)
        {
            return (DeckModel.RemainingCardsCount - playerCount) < playerCount;
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