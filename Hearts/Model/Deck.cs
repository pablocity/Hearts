using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibUtility;

namespace Hearts.Model
{
    public class Deck
    {
        //TODO Test code
        public List<Card> cards = new List<Card>();

        public Deck()
        {
            for (int i = 0; i < Enum.GetNames(typeof(Values)).Length; i++)
            {
                for (int j = 0; j < Enum.GetNames(typeof(Suits)).Length; j++)
                {
                    Card cardToAdd = new Card((Suits)j, (Values)i);
                    cards.Add(cardToAdd);
                }
            }
        }

        public Deck(List<Card> cardsToAdd)
        {
            cards = cardsToAdd;
        }

        

        public void Shuffle(Random random)
        {
            List<Card> temporaryCards = new List<Card>();

            while (cards.Count > 0)
            {
                int index = random.Next(0, cards.Count);
                temporaryCards.Add(cards[index]);
                cards.RemoveAt(index);
            }

            cards = temporaryCards;
        }
    }
}
