namespace Models
{
    public enum Suits
    {
        Clubs = 1,
        Diamonds,
        Hearts,
        Spades
    }

    public enum Ranks
    {
        Ace = 1, 
        Two, Three, Four, Five, Six,
        Seven, Eight, Nine, Ten, Jack, Queen, King
    }
    
    public class Card
    {
        public Ranks Rank { get; }
        public Suits Suit { get; }
        public float Weight { get; }

        public Card(Ranks rank, Suits suit, float weight)
        {
            Rank = rank;
            Suit = suit;
            Weight = weight;
        }
        
        public override string ToString()
        {
            return $"{Rank} of {Suit}\n(Weight: {Weight:F1})";
        }
    }
}