using System.Collections.Generic;

namespace Models
{
    public class Player
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
        public int Score { get; private set; }
        private Stack<Card> Hand { get; set; }
        
        public Player(string name, int id)
        {
            Name = name;
            Id = id;
            Score = 0;
            Hand = new Stack<Card>();
        }

        public void AddCard(Card card)
        {
            Hand.Push(card);
        }

        public void AddScore(int score)
        {
            Score += score;
        }

        public Card GetLastCard()
        {
            if (Hand.Count == 0)
            {
                return null;
            }

            return Hand.Peek();
        }

        public void Reset()
        {
            Score = 0;
            Hand.Clear();
        }
    }
}