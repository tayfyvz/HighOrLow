using Models;

namespace Utils
{
    public static class CardComparer
    {
        public static int Compare(Card a, Card b)
        {
            if (a.Rank != b.Rank)
            {
                return a.Rank.CompareTo(b.Rank);
            }

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