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

        public async void StartGame()
        {
            deck.Shuffle();
            //TODO remove test case
            if (Players.Count == 3)
                deck.RemoveReduntant();
            else if (Players.Count != 4)
                return;

            int deckCount = deck.cards.Count;
            foreach (ClientHandler cl in Players)
            {

                for (int i = 0; i < deckCount/Players.Count; i++)
                {
                    cl.PlayerStats.Hand.Add(deck.Deal());
                }
            }

            foreach (ClientHandler cl in Players)
            {
                await cl.SendData(new Message(MessageType.ShowCards, null, cl.PlayerStats.Hand.ToArray()));
            }

            foreach (ClientHandler cl in Players)
            {
                Message response = await cl.SendData(new Message(MessageType.PassOn, null));

            }


            //TODO zmienić kolejność przekazywania kart po ukończeniu rozgrywki
            for (int i = 0; i < Players.Count; i++)
            {
                Message response = await Players[i].SendData(new Message(MessageType.PassOn, null));
                
                if (i + 1 <= Players.Count-1)
                {
                    Players[i + 1].PlayerStats.Hand.AddRange(response.CardsRequested);
                }
                else
                {
                    Players[Players.Count - (i + 1)].PlayerStats.Hand.AddRange(response.CardsRequested);
                }

                await Players[i].SendData(new Message(MessageType.ShowCards, Players[i].PlayerStats, null));

            }

            //List<Card> testList = new List<Card>();

            //foreach (ClientHandler cl in Players)
            //{
            //    testList = cl.PlayerStats.hand;
            //}
        }
    }
}
