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

        private Message serverOrder;

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

        private ObservableCollection<Card> cardsInHand;
        public ObservableCollection<Card> CardsInHand
        {
            get
            {
                return cardsInHand;
            }
            private set
            {
                cardsInHand = value;
                RaisePropertyChanged(() => CardsInHand);
            }
        }

        
        public ObservableCollection<string> Messages { get; set; }

        public RelayCommand passCards { get; private set; }
        public RelayCommand<Card> selectCard { get; private set; }

        public GameViewModel()
        {
            
            Stats = new Player("");
            Messages = new ObservableCollection<string>();
            cardsInHand = new ObservableCollection<Card>();
            passCards = new RelayCommand(PassOn);
            selectCard = new RelayCommand<Card>(Select);

            Messenger.Default.Register<Message>(this, RetrieveMessage);
            
        }

        private void RetrieveMessage(Message serverRequest)
        {
            
            //Messages.Add("Client configured");
            serverOrder = serverRequest;

            switch (serverRequest.Request)
            {
                case MessageType.CardRequest:
                    ShowCards(serverRequest);
                    break;
                case MessageType.ShowCards:
                    ShowCards(serverRequest);
                    break;
                case MessageType.PassOn:
                    Inform("Pass on");
                    break;
            }
        }


        public async Task<Message> SelectCard(Message request)
        {
            return new Message(MessageType.Error, null, null);
            //Check if player has selected a proper card
            //return new Message(MessageType.CardRequest, null, new Card(Suits.Diamonds, Values.Seven));
        }

        public void ShowCards(Message message)
        {
            List<Card> hand = new List<Card>();

            foreach (Card c in message.CardsRequested)
            {
                hand.Add(c);
            }

            Stats.Hand = hand;
            CardsInHand = new ObservableCollection<Card>(hand);

            Inform("Odświeżono karty");
            
        }

        public void PassOn()
        {
            if (CanBePassed())
            {
                if (Stats.SelectedCards.Count == 3)
                {
                    Inform("Karty wysłane");
                    clientInstance.SendData(new Message(MessageType.PassOn, null, Stats.SelectedCards.ToArray()));
                    Stats.Hand = Stats.Hand.Except(Stats.SelectedCards).ToList();
                    Stats.SelectedCards.Clear();
                    Stats.PassOrSelect = false;
                    CardsInHand = new ObservableCollection<Card>(Stats.Hand);
                }
                else
                {
                    Inform("Musisz wybrać 3 karty!");
                }
            }
        }

        private bool CanBePassed()
        {
            if (!IsInDesignMode)
            {
                if (serverOrder != null && serverOrder.Request == MessageType.PassOn)
                    return true;
                else
                    return false;
            }
            else
                return false;
            
        }

        //TODO check equality comparison
        private void Select(Card toSelect)
        {
            Inform("Select");
            if (Stats.PassOrSelect)
            {
                if (Stats.SelectedCards.Contains(toSelect))
                {
                    Stats.SelectedCards.Remove(toSelect);
                    Inform($"Usunięto {toSelect.ToString()}");
                }
                else if (!Stats.SelectedCards.Contains(toSelect) && Stats.SelectedCards.Count < 3)
                {
                    Stats.SelectedCards.Add(toSelect);
                    Inform($"Dodano {toSelect.ToString()}");
                }
                else
                {
                    Card lastOne = Stats.SelectedCards[Stats.SelectedCards.Count - 1];
                    Stats.SelectedCards[Stats.SelectedCards.Count - 1] = toSelect;
                    Inform($"Wymieniono {lastOne} na {toSelect.ToString()}");
                }

            }
            else
            {
                Stats.SelectedCards.Clear();
                Stats.SelectedCards.Add(toSelect);
            }
        }


        private void Inform(string informMessage)
        {
            Console.WriteLine(informMessage);
        }
        
    }
}
