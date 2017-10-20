using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.Model
{
    public enum MessageType
    {
        Win, //indicates player has won a game
        Lose, // indicates player has lost the game
        CardRequest, //demands a specific card
        PassOn, //it is a command to pass on three cards
        YourTurn, //indicates player's turn
        YouDeal, //informs player that he is dealing in this round
        ShowCards, //it shows updated cards in hand
        ShowPot, //it shows current pot of cards
        Error //shows an error that occured
    }

    public class Message
    {
        public MessageType Request { get; private set; }
        public List<Card> CardsRequested { get; private set; }

        public Player PlayerStats { get; private set; }

        public Suits? Suit { get; private set; } = null;

        //TODO check Game.HeartsAllowed after each round
        public bool HeartsAllowed { get; set; }

        public Message(MessageType request, Player player, params Card[] card)
        {
            PlayerStats = player;
            CardsRequested = new List<Card>();
            Request = request;

            if (card != null)
                CardsRequested.AddRange(card);

            if (request == MessageType.YourTurn)
            {
                Suit = card[0].Suit;
            }

        }

        //TODO dodać ToString()
        public override string ToString()
        {

            string description = $"Request type: {Request}\nRequested card(s):\n";

            if (CardsRequested.Count > 1)
            {
                foreach (Card card in CardsRequested)
                {
                    description += $"{card.Name}\n";
                }
            }
            else
            {
                description += $"{CardsRequested[0]}\n";
            }


            description += (Suit != null) ? $"Suit: {Suit}\n" : "";

            description += $"Hearts allowed: {HeartsAllowed}";

            return description;
   
        }
    }
}
