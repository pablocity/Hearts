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
        public List<ClientHandler> Players { get; set; }

        public static bool HeartsAllowed = false;

        public static Game Instance
        {
            get
            {
                return instance;
            }
        }


        private static readonly Game instance = new Game();

        private ClientHandler dealer
        {
            get
            {
                return Players[0];
            }
        }

        private bool isEnd = false;
        private static Deck deck;

        static Game()
        {
            deck = new Deck();
        }
        private Game()
        {
            
        }


        public async Task StartGame()
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
                await UpdateCards(UpdateContent.Hand);

                //TODO zmienić kolejność przekazywania kart po ukończeniu rozgrywki
                for (int i = 0; i < Players.Count; i++)
                {
                    Message response = await Players[i].SendData(new Message(MessageType.PassOn, null));
                    //Players[i].PlayerStats.Hand = Players[i].PlayerStats.Hand.Except(response.CardsRequested).ToList();

                    foreach (Card c in response.CardsRequested)
                    {
                        for (int j = 0; j < Players[i].PlayerStats.Hand.Count; j++)
                        {
                            if (c.Equals(Players[i].PlayerStats.Hand[j]))
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

                    await Players[i].SendData(new Message(MessageType.ShowCards, null, Players[i].PlayerStats.Hand.ToArray()), false);

                }
                //TODO REFACTOR
                await UpdateCards(UpdateContent.Hand);

                await PlayFirstRound();

                int x = 0;
                while (!isEnd)
                {
                    x++;
                    // Test log
                    Console.WriteLine($"New round: {x}");
                    //
                    await PlayRound();

                    ClientHandler winner = CheckIfOver();

                    if (winner != null)
                    {
                        isEnd = true;
                        await winner.SendData(new Message(MessageType.Win, null, null), false);
                    }
                    
                }

                Console.WriteLine("KONIEC GRY!");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        // Returns a winner if any
        private ClientHandler CheckIfOver()
        {
            if (Players[0].PlayerStats.Hand.Count <= 0)
            {
                Dictionary<ClientHandler, int> maxPoints = new Dictionary<ClientHandler, int>();
                foreach (ClientHandler cl in Players)
                {
                    maxPoints.Add(cl, cl.PlayerStats.Points);
                }

                return maxPoints.FirstOrDefault(x => x.Value == maxPoints.Values.Min()).Key;
            }
            else
                return null;
        }

        private async Task PlayFirstRound()
        {
            foreach (ClientHandler cl in Players)
            {
                //REMOVE player's stats from message
                Message clientCard = await cl.SendData(new Message(MessageType.CardRequest, null, new Card(Suits.Clubs, Values.Two)));
                
                cl.PlayerStats.CurrentCard = clientCard.CardsRequested.Count > 0 ? clientCard.CardsRequested[0] : null;
            }

            //TODO check if works
            await UpdateCards(UpdateContent.Pot);

            // Test log
            Console.WriteLine("Play first round there should be no two of clubs");
            //
            await UpdateQueueOrder(true);

            await PlayRound(true);

        }


        //TODO check if player has to give two of clubs

        private async Task PlayRound(bool firstRound = false)
        {

            Message dealCard = null;

            if (!firstRound)
            {
                Message toSend = new Message(MessageType.YouDeal, null, null)
                {
                    HeartsAllowed = Game.HeartsAllowed
                };

                dealCard = await dealer.SendData(toSend);

                //REFACTOR

                foreach (ClientHandler cl in Players)
                {
                    cl.PlayerStats.CurrentCard = null;
                }

                dealer.PlayerStats.CurrentCard = dealCard.CardsRequested[0];
                await UpdateCards(UpdateContent.Pot);
                //await UpdatePlayersStats();
            }


            for (int i = 1; i < Players.Count; i++)
            {
                Message toSend = new Message(MessageType.YourTurn, null, dealer.PlayerStats.CurrentCard)
                {
                    HeartsAllowed = Game.HeartsAllowed
                };

                dealCard = await Players[i].SendData(toSend);
                Players[i].PlayerStats.CurrentCard = dealCard.CardsRequested[0];

                await UpdateCards(UpdateContent.Pot);
            }

            //Some kind of timer to delay collecting last pot

            await UpdateQueueOrder(false);

        }


        //Assessing pot, returns winning card
        private Card Assess()
        {
            Card cardDealt = dealer.PlayerStats.CurrentCard;
            Card max = cardDealt;

            Card[] pot = Pot().ToArray();

            foreach (Card p in pot)
            {
                if (p.Equals(cardDealt))
                    continue;

                if (p.Suit == cardDealt.Suit)
                {
                    Card[] temp = { p, max };
                    max = temp.Where(x => x.Value == temp.Max(y => y.Value)).ToArray()[0];
                }
            }

            foreach (ClientHandler cl in Players)
            {
                if (cl.PlayerStats.CurrentCard.Equals(max))
                {
                    cl.PlayerStats.Garbage.AddRange(pot);

                    //cl.SendData(new Message(MessageType.ShowStats, cl.PlayerStats, null), false);

                    return cl.PlayerStats.CurrentCard;
                }
            }

            return null;

        }

        private async Task UpdateQueueOrder(bool firstRound)
        {

            List<ClientHandler> temporary = new List<ClientHandler>();

            Card bestCard = firstRound ? null : Assess();

            await UpdatePlayersStats();

            for (int i = 0; i < Players.Count; i++)
            {
                if (firstRound)
                {
                    if (Players[i].PlayerStats.CurrentCard != null &&
                        Players[i].PlayerStats.CurrentCard.Value == Values.Two &&
                        Players[i].PlayerStats.CurrentCard.Suit == Suits.Clubs)

                        break;
                }
                else
                {
                    if (Players[i].PlayerStats.CurrentCard.Equals(bestCard))
                        break;
                }

                temporary.Add(Players[i]);

                //TODO check if works
                Players[i].PlayerStats.CurrentCard = null;
            }

            if (temporary.Count > 0)
            {
                Players.RemoveRange(0, temporary.Count);
                Players.AddRange(temporary);
                temporary = null;
            }

        }


        private enum UpdateContent
        {
            Pot,
            Hand,
            Both
        }

        private async Task UpdateCards(UpdateContent toUpdate)
        {
            foreach (ClientHandler cl in Players)
            {
                if (toUpdate == UpdateContent.Hand)
                    await cl.SendData(new Message(MessageType.ShowCards, null, cl.PlayerStats.Hand.ToArray()), false);
                else if (toUpdate == UpdateContent.Pot)
                    await cl.SendData(new Message(MessageType.ShowPot, cl.PlayerStats, Pot().ToArray()), false);
                else
                {
                    await UpdateCards(UpdateContent.Hand);
                    await UpdateCards(UpdateContent.Pot);
                }

            }
        }

        private async Task UpdatePlayersStats()
        {
            foreach (ClientHandler cl in Players)
            {
                //TODO check if works
                await cl.SendData(new Message(MessageType.ShowStats, cl.PlayerStats, null), false);
            }
        }

        private IEnumerable<Card> Pot()
        {
            List<Card> cards = new List<Card>();
            Card currentCard;

            for (int i = 0; i < Players.Count; i++)
            {
                currentCard = Players[i].PlayerStats.CurrentCard;

                if (currentCard != null)
                    cards.Add(currentCard);
            }

            return cards;
        }
    }
}
