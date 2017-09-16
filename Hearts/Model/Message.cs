﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.Model
{
    public enum MessageType
    {
        Win,
        Lose,
        CardRequest,
        YourTurn,
        ShowCards,
        Error
    }

    public class Message
    {
        public MessageType Request { get; private set; }
        public List<Card> CardsRequested { get; private set; }

        public Message(MessageType request, params Card[] card)
        {
            CardsRequested = new List<Card>();
            Request = request;

            if (card != null)
                CardsRequested.AddRange(card);

        }

        //TODO dodać ToString()
    }
}
