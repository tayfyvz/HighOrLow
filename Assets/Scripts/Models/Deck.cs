using System;
using System.Collections.Generic;
using Controllers;
using Utils;

namespace Models
{
    /// <summary>
    /// Manages a deck of 52 cards using a weighted random selection for drawing cards.
    /// Uses CardsWeightConfig to determine each card's weight.
    /// </summary>
    public class Deck
    {
        public int RemainingCardsCount => _cards.Count;
        
        private readonly IRandomNumberGenerator _rng;
    
        private List<Card> _cards;
    
        private float _totalWeight;
    
        // Dependency injection is used here to pass weight configuration.
        /// <param name="weightConfig">The configuration object that provides card weights.</param>
        /// <param name="rng">A custom random number generator; if null, a SystemRandomNumberGenerator will be used.</param>
        public Deck(IRandomNumberGenerator rng = null)
        {
            _rng = rng ?? new SystemRandomNumberGenerator();
        
            Reset();
        }

        // Initializes (or re-initializes) the deck with 52 unique cards.
        public void Reset()
        {
            Suits[] suits = (Suits[])Enum.GetValues(typeof(Suits));
            Ranks[] ranks = (Ranks[])Enum.GetValues(typeof(Ranks));
        
            _cards = new List<Card>(suits.Length * ranks.Length);
            _totalWeight = 0;
        
            foreach (Suits suit in suits)
            {
                foreach (Ranks rank in ranks)
                {
                    float weight = SettingsController.Instance.GetCardWeight(suit, rank);
                    _cards.Add(new Card(rank, suit, weight));
                    _totalWeight += weight;
                }
            }
        }

        public Card DrawCard()
        {
            if (_cards.Count == 0)
            {
                return null;
            }

            // Generate a random float between 0 and the total weight.
            float randomValue = (float)(_rng.NextDouble() * _totalWeight);
            float minTargetWeight = 0f;
        
            for (int i = 0; i < _cards.Count; i++)
            {
                minTargetWeight += _cards[i].Weight;
                if (minTargetWeight > randomValue)
                {
                    Card selectedCard = _cards[i];
                
                    _cards.RemoveAt(i);
                    _totalWeight -= selectedCard.Weight;
                
                    return selectedCard;
                }
            }

            return null;
        }
        
        public bool HasEnoughCards(int count)
        {
            return _cards.Count >= count;
        }
    }
}