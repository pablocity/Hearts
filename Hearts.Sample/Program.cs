using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearts.Model;

namespace Hearts.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Deck deck = new Deck();

            deck.Shuffle(new Random());

            foreach (Card card in deck.cards)
            {
                Console.WriteLine(card.ToString());
            }

            Console.ReadKey();
        }
    }
}
