using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Hearts.Model;
using System.Collections.ObjectModel;

namespace Hearts.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        //Game instance

        public bool Ready = false;

        public Player Stats;

        Client clientInstance;
        public Client ClientInstance
        {
            get
            {
                return clientInstance;
            }
            set
            {
                Set<Client>(() => ClientInstance, ref clientInstance, value);
            }
        }

        public ObservableCollection<Card> cardsInHand;

        public RelayCommand passCards;

        public GameViewModel()
        {
            Messenger.Default.Register<Message>(this, RetrieveMessage);
            cardsInHand = new ObservableCollection<Card>();
            passCards = new RelayCommand(() => 
            {
                if (Stats.SelectedCards.Count == 3)
                    Ready = true;
            });
        }


        private async Task<Message> SelectCard(Message request)
        {
            //Check if player has selected a proper card
            //return new Message(MessageType.CardRequest, null, new Card(Suits.Diamonds, Values.Seven));
        }

        private async Task<Message> ShowCards()
        {

        }

        private async Task<Message> PassOn()
        {
            //while ()
        }

        //TODO check equality comparison
        private void Select(Card toSelect)
        {
            if (Stats.PassOrSelect)
            {
                if (Stats.SelectedCards.Contains(toSelect))
                {
                    Stats.SelectedCards.Remove(toSelect);
                }
                else if (!Stats.SelectedCards.Contains(toSelect) && Stats.SelectedCards.Count < 3)
                {
                    Stats.SelectedCards.Add(toSelect);
                }
                else
                {
                    Stats.SelectedCards[Stats.SelectedCards.Count - 1] = toSelect;
                }

            }
            else
            {
                Stats.SelectedCards.Clear();
                Stats.SelectedCards.Add(toSelect);
            }
        }
    }
}
