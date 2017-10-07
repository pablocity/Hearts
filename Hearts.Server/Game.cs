﻿using System;
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
            
        }

        public async void StartGame()
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
                    cl.PlayerStats.Hand.Add(deck.Deal());
                }
            }

            try
            {
                await UpdateCards();

                //Message response = await cl.SendData(new Message(MessageType.PassOn, null));

                //TODO zmienić kolejność przekazywania kart po ukończeniu rozgrywki
                for (int i = 0; i < Players.Count; i++)
                {
                    Message response = await Players[i].SendData(new Message(MessageType.PassOn, null));
                    //Players[i].PlayerStats.Hand = Players[i].PlayerStats.Hand.Except(response.CardsRequested).ToList();

                    foreach (Card c in response.CardsRequested)
                    {
                        for (int j = 0; j < Players[i].PlayerStats.Hand.Count; j++)
                        {
                            if (c.Name == Players[i].PlayerStats.Hand[j].Name)
                                Players[i].PlayerStats.Hand.Remove(Players[i].PlayerStats.Hand[j]);
                        }
                    }

                    int index = 0;

                    if (i + 1 <= Players.Count - 1)
                    {
                        index = i + 1;
                        Players[index].PlayerStats.Hand.AddRange(response.CardsRequested);
                    }
                    else
                    {
                        index = Players.Count - (i + 1);
                        Players[index].PlayerStats.Hand.AddRange(response.CardsRequested);
                    }

                    await Players[i].SendData(new Message(MessageType.ShowCards, /*Players[i].PlayerStats*/null, Players[i].PlayerStats.Hand.ToArray()), false);
                    //await Players[index].SendData(new Message(MessageType.ShowCards, null, Players[i].PlayerStats.Hand.ToArray()), false);

                }
                //TODO REFACTOR
                await UpdateCards();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        private async Task UpdateCards()
        {
            foreach (ClientHandler cl in Players)
            {
                //TODO remember aobut disabling response in showCards message
                await cl.SendData(new Message(MessageType.ShowCards, null, cl.PlayerStats.Hand.ToArray()), false);
            }
        }
    }
}
