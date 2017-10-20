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

        public static bool HeartsAllowed = false;

        private static readonly Game instance = new Game();
        public static Game Instance
        {
            get
            {
                return instance;
            }
        }
        public List<ClientHandler> Players { get; set; }

        private ClientHandler dealer;

        private static Deck deck;

        static Game()
        {
            deck = new Deck();
        }
        private Game()
        {
            
        }

        bool isEnd = false;

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
                            if (c == Players[i].PlayerStats.Hand[j])
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

                }
                //TODO REFACTOR
                await UpdateCards(UpdateContent.Hand);

                await PlayFirstRound();

                //while (!isEnd)
                //{

                //}


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        private async Task PlayFirstRound()
        {
            foreach (ClientHandler cl in Players)
            {
                Message clientCard = await cl.SendData(new Message(MessageType.CardRequest, null, new Card(Suits.Clubs, Values.Two)));
                cl.PlayerStats.CurrentCard = clientCard.CardsRequested[0];
            }

            //int starterIndex = -1;

            //List<ClientHandler> temporary = new List<ClientHandler>();

            //for (int i = 0; i < Players.Count; i++)
            //{
            //    if (Players[i].PlayerStats.CurrentCard != null &&
            //        Players[i].PlayerStats.CurrentCard.Value == Values.Two &&
            //        Players[i].PlayerStats.CurrentCard.Suit == Suits.Clubs)
            //    {
            //        starterIndex = i;
            //        break;
            //    }

            //    if (starterIndex < i)
            //        temporary.Add(Players[i]);
            //}

            //if (temporary.Count > 0)
            //{
            //    Players.RemoveRange(0, temporary.Count);
            //    Players.AddRange(temporary);
            //    temporary = null;
            //}

            //dealer = Players[0];


            UpdateQueueOrder((c) => { if (c != null && c.Value == Values.Two && c.Suit == Suits.Clubs) return true; else return false; });


        }


        //TODO check if player has to give two of clubs

        private async void PlayRound()
        {
            if (dealer != Players[0])
                dealer = Players[0];


            //TODO check if HeartsAllowed has changed
            Message dealCard = await dealer.SendData(new Message(MessageType.YouDeal, null, null));
            dealer.PlayerStats.CurrentCard = dealCard.CardsRequested[0];

            await UpdateCards(UpdateContent.Pot);

            for (int i = 1; i < Players.Count; i++)
            {
                dealCard = await Players[i].SendData(new Message(MessageType.YourTurn, null, dealer.PlayerStats.CurrentCard));
                Players[i].PlayerStats.CurrentCard = dealCard.CardsRequested[0];

                await UpdateCards(UpdateContent.Pot);
            }

            //Some kind of timer to delay collecting last pot

            Assess();

        }

        private Card Assess()
        {
            //TODO call UpdateQueueOrder somewhere
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
                    max = temp.Max();
                }
            }

            foreach (ClientHandler cl in Players)
            {
                if (cl.PlayerStats.CurrentCard.Equals(max))
                {
                    cl.PlayerStats.garbage.AddRange(pot);
                    return cl.PlayerStats.CurrentCard;
                }
            }

            return null;

        }

        private void UpdateQueueOrder(Predicate<Card> condition)
        {
            //int starterIndex = -1;

            List<ClientHandler> temporary = new List<ClientHandler>();

            for (int i = 0; i < Players.Count; i++)
            {
                //if (Players[i].PlayerStats.CurrentCard != null &&
                //    Players[i].PlayerStats.CurrentCard.Value == Values.Two &&
                //    Players[i].PlayerStats.CurrentCard.Suit == Suits.Clubs)
                //{
                //    starterIndex = i;
                //    break;
                //}

                if (condition(Players[i].PlayerStats.CurrentCard))
                    break;

                temporary.Add(Players[i]);
            }

            if (temporary.Count > 0)
            {
                Players.RemoveRange(0, temporary.Count);
                Players.AddRange(temporary);
                temporary = null;
            }

            dealer = Players[0];
            //TODO set dealer
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
                    await cl.SendData(new Message(MessageType.ShowPot, null, Pot().ToArray()), false);
                else
                {
                    await UpdateCards(UpdateContent.Hand);
                    await UpdateCards(UpdateContent.Pot);
                }
                //TODO remember aobut disabling response in showCards message

            }
        }

        private IEnumerable<Card> Pot()
        {
            Card[] cards = new Card[Players.Count];

            for (int i = 0; i < Players.Count; i++)
            {
                cards[0] = Players[i].PlayerStats.CurrentCard;
            }

            return cards;
        }
    }
}
