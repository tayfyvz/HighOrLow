using System.Collections.Generic;

namespace Models
{
    /// <summary>
    /// Represents a single player.
    /// </summary>
    public class Player
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
        public int Score { get; private set; }
        public List<Card> Hand { get; private set; }
        
        public Player(string name, int id)
        {
            Name = name;
            Id = id;
            Score = 0;
            Hand = new List<Card>();
        }

        public void AddCard(Card card)
        {
            Hand.Add(card);
        }

        public void AddScore(int score)
        {
            Score += score;
        }
    }
}