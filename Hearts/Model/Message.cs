using System;
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

        public Message(MessageType request, params Card[] cards)
        {
            Request = request;
            CardsRequested = cards.ToList();
        }
    }
}
