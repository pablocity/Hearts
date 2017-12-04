using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.Model
{
    public class Player
    {

        public bool PassOrSelect;

        public List<Card> Hand;
        public List<Card> SelectedCards;
        public List<Card> Garbage;
        //public bool IsDealer = false;

        public string Name { get; private set; }


        private Card currentCard;
        public Card CurrentCard
        {
            get
            {
                return currentCard;
            }
            set
            {
                //TODO check if setter removes played card
                currentCard = value;
                Hand.Remove(value);
            }
        }

        // Points amount based on number of garbage cards
        private int points;
        public int Points
        {
            get
            {
                if (Garbage != null && Garbage.Count > 0)
                {
                    points = 0;

                    foreach (Card card in Garbage)
                    {
                        points += card.Points;
                    }
                }
                
                return points;
            }
        }

        public Player(string name)
        {
            Garbage = new List<Card>();
            PassOrSelect = true;
            Hand = new List<Card>();
            SelectedCards = new List<Card>();
            Name = name;
        }


        public Card SelectCard(int index)
        {
            return Hand[index];
        }

    }
}
