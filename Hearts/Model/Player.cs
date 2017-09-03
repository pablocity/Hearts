using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.Model
{
    public class Player
    {
        public string Name;
        public List<Card> hand;
        public Card selectedCard;

        public Player(string name)
        {
            hand = new List<Card>();
            Name = name;
        }

        public Card SelectCard(int index)
        {
            return hand[index];
        }

    }
}
