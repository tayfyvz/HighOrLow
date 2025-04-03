using Models;

namespace Utils
{
    namespace Utils
    {
    }

    public static class CardComparer
    {
        /// <summary>
        /// Compares two cards. Returns a positive number if a is greater than b.
        /// Comparison is based on the card’s numeric value (rank). If equal, suit priority is used.
        /// Suit priority: Spades (4) > Hearts (3) > Diamonds (2) > Clubs (1).
        /// </summary>
        public static int Compare(Card a, Card b)
        {
            if (a.Rank != b.Rank)
                return a.Rank.CompareTo(b.Rank);
            else
                return GetSuitPriority(a.Suit).CompareTo(GetSuitPriority(b.Suit));
        }

        private static int GetSuitPriority(Suits suit)
        {
            switch (suit)
            {
                case Suits.Spades: return 4;
                case Suits.Hearts: return 3;
                case Suits.Diamonds: return 2;
                case Suits.Clubs: return 1;
                default: return 0;
            }
        }
    }
}