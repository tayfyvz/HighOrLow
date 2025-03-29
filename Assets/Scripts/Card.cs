public enum Suits
{
    None = 0,
    Clubs,
    Diamonds,
    Hearts,
    Spades
}

public enum Ranks
{
    None = 0, 
    Ace, Two, Three, Four, Five, Six,
    Seven, Eight, Nine, Ten, Jack, Queen, King
}

public class Card
{
    public Ranks Ranks { get; private set; }
    public Suits Suit { get; private set; }
    public float Weight { get; private set; }

    public Card(Ranks ranks, Suits suit, float weight)
    {
        Ranks = ranks;
        Suit = suit;
        Weight = weight;
    }
}