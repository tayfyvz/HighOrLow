using Models;
using UnityEngine;
using Utils;
using Views;
using Logger = Utils.Logger;

namespace Controllers
{
    public class DeckController : BaseController<IDeckView>, IDeckController
    {
        private Deck DeckModel { get; set; }

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

            Logger.Log("Deck controller view is null.", LogType.Error);
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