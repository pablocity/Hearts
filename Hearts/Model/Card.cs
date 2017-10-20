using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Resources;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

namespace Hearts.Model
{
    public enum Suits
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
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

        public int Points;

        public string Name;

        public bool Replaceable;

        public string ImgString { get; private set; }
        

        public Card(Suits suit, Values value, bool isReplaceable = true)
        {
            Suit = suit;
            Value = value;

            Name = String.Format("{0} of {1}", value, suit);
            Replaceable = isReplaceable;

            Replaceable = CheckIfReplaceable(this);
            Points = CalculatePoints(this);
            //TODO some try catch code
            //TODO adjust to screen size
            
            ImgString = $@"/Resources/{Name}.png";

        }

        public static int CalculatePoints(Card card)
        {
            if (card.Suit == Suits.Hearts)
                return 1;
            else if (card.Suit == Suits.Spades && card.Value == Values.Queen)
                return 13;
            else
                return 0;
        }

        public static bool CheckIfReplaceable(Card cardToReplace)
        {
            if (cardToReplace.Suit == Suits.Hearts || (cardToReplace.Suit == Suits.Clubs && cardToReplace.Value == Values.Two)
                || (cardToReplace.Suit == Suits.Spades && cardToReplace.Value == Values.Queen))
            {
                return false;
            }
            else
                return true;
        }


        public override bool Equals(object obj)
        {
            Card card = obj is Card ? obj as Card : null;

            if (card != null && Name == card.Name)
                return true;
            else
                return false;
        }

        public static bool operator ==(Card c1, Card c2)
        {
            if (c1.Equals(c2))
                return true;
            else
                return false;
        }

        public static bool operator !=(Card c1, Card c2)
        {
            if (!c1.Equals(c2))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} replaceable: {1}, points: {2}", Name, Replaceable, Points);
        }
    }
}
