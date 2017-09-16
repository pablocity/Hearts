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

        private static Game instance = null;
        public static Game Instance
        {
            get
            {
                if (instance == null)
                    instance = new Game();

                return instance;
            }
        }
        public List<ClientHandler> Players { get; set; }

        private Deck deck;

        private Game()
        {
            deck = new Deck();
        }

        public void StartGame()
        {
            if (Players.Count != 3 && Players.Count != 4)
                return;

            deck.Shuffle();

            if (Players.Count == 3) deck.RemoveReduntant();

            foreach (ClientHandler cl in Players)
            {
                for (int i = 0; i < deck.cards.Count/Players.Count; i++)
                {
                    cl.PlayerStats.hand.Add(deck.Deal());
                }
            }


            foreach (ClientHandler cl in Players)
            {

            }
        }
    }
}
