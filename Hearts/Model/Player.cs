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
        public List<Card> Hand;
        public List<Card> SelectedCards;
        public Card CardToSend;
        public bool PassOrSelect;
        public bool IsDealer = false;

        private List<Card> garbage;
        //TODO finish token, wysyłany jako messagae object w celu powiadmoienia serwera o zakończeniu pojedynczego zleconego zadania

        private int points;
        public int Points
        {
            get
            {
                //TODO jeśli ilość kart jest taka sama nie sprawdzaj
                if (garbage != null && garbage.Count > 0)
                {
                    points = 0;

                    foreach (Card card in garbage)
                    {
                        points += card.Points;
                    }
                }
                
                return points;
            }
        }

        public Player(string name)
        {
            garbage = new List<Card>();
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
