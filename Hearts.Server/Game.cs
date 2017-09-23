using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearts.Model;

namespace Hearts.Server
{
    public class Game
    {

        private static readonly Game instance = new Game();
        public static Game Instance
        {
            get
            {
                return instance;
            }
        }
        public List<ClientHandler> Players { get; set; }

        private static Deck deck;

        static Game()
        {
            deck = new Deck();
        }
        private Game()
        {
            //TODO check client's memory leak
        }

        public void StartGame()
        {
            deck.Shuffle();
            //TODO remove test case
            if (Players.Count == 3)
                deck.RemoveReduntant();
            else if (Players.Count != 2)
                return;

            int deckCount = deck.cards.Count;
            foreach (ClientHandler cl in Players)
            {

                for (int i = 0; i < deckCount/Players.Count; i++)
                {
                    cl.PlayerStats.hand.Add(deck.Deal());
                }
            }

            List<Card> testList = new List<Card>();

            foreach (ClientHandler cl in Players)
            {
                testList = cl.PlayerStats.hand;
            }
        }
    }
}
