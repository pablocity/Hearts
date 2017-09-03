using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.Model
{
    public enum Suits
    {
        Spades,
        Clubs,
        Diamonds,
        Hearts
    }

    public enum Values
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Card
    {
        public Suits Suit;
        public Values Value;

        public string Name;

        public bool Replaceable;


        public Card(Suits suit, Values value, bool isReplaceable = true)
        {
            Suit = suit;
            Value = value;

            Name = String.Format("{0} of {1}", value, suit);
            Replaceable = isReplaceable;
        }

        public override string ToString()
        {
            return String.Format("{0} replaceable: {1}", Name, Replaceable);
        }
    }
}
