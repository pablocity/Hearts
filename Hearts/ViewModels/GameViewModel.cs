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

        public Player Stats;

        private Message serverOrder;

        private bool heartsChanged = false;

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

        private ObservableCollection<string> messages;
        public ObservableCollection<string> Messages
        {
            get
            {
                return messages;
            }
            private set
            {
                messages = value;
                RaisePropertyChanged(() => Messages);
            }
        }

        public RelayCommand passCards { get; private set; }
        public RelayCommand sendCard { get; private set; }
        public RelayCommand<Card> selectCard { get; private set; }

        public GameViewModel()
        {
            
            Stats = new Player("");
            Messages = new ObservableCollection<string>();
            cardsInHand = new ObservableCollection<Card>();
            passCards = new RelayCommand(PassOn);
            sendCard = new RelayCommand(SendCard);
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
                    Inform($"Server requested: {serverOrder.CardsRequested[0]}");
                    SendRequestedCard(serverOrder.CardsRequested[0]);
                    break;
                case MessageType.ShowCards:
                    ShowCards(serverRequest);
                    break;
                case MessageType.PassOn:
                    Inform("Pass on");
                    break;
                case MessageType.ShowPot:
                    Inform($"Pot contains: {serverOrder.CardsRequested}");
                    ShowPot(serverOrder.CardsRequested);
                    Inform($"\nPunkty: {serverOrder.PlayerStats.Points}\n");
                    break;
                case MessageType.YouDeal:
                    Inform($"You deal now!");
                    break;
                case MessageType.YourTurn:
                    Inform("Your turn now!");
                    break;
                case MessageType.ShowStats:
                    //Inform($"\nPunkty: {serverOrder.PlayerStats.Points}\n");
                    break;
            }
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

        public void ShowPot(IEnumerable<Card> pot)
        {
            List<string> names = new List<string>();

            foreach (Card c in pot)
            {
                names.Add(c.Name);
            }
            Messages = new ObservableCollection<string>(names);
        }

        private bool CheckSelectedCard(Card selectedCard)
        {

            if (serverOrder.Request == MessageType.YouDeal)
            {
                if (selectedCard.Suit == Suits.Hearts && !serverOrder.HeartsAllowed)
                {
                    foreach (Card c in Stats.Hand)
                    {
                        if (c.Suit != Suits.Hearts)
                        {
                            Inform("Nie możesz jeszcze tego wyłożyć, gdyż posiadasz inne kolory a nikt wcześniej nie zagrał serca!");
                            return false;
                        }
                        
                    }

                    heartsChanged = true;

                    return true;
                }

                return true;
            }


            if (serverOrder.Request == MessageType.CardRequest)
            {
                if (serverOrder.CardsRequested[0].Equals(selectedCard))
                    return true;
                else
                {
                    foreach (Card c in Stats.Hand)
                    {
                        if (c.Equals(serverOrder.CardsRequested[0]))
                        {
                            Inform($"Wybierz wymaganą kartę: {serverOrder.CardsRequested[0].Name}");
                            return false;
                        }
                    }
                    
                    return true;
                }
            }

            if (serverOrder.Request == MessageType.YourTurn)
            {
                if (serverOrder.CardsRequested[0].Suit.Equals(selectedCard.Suit))
                    return true;
                else
                {
                    foreach (Card card in Stats.Hand)
                    {
                        if (card.Suit == serverOrder.CardsRequested[0].Suit)
                        {
                            Inform("Posiadasz wymagany kolor!");
                            return false;
                        }
                    }

                    if (!serverOrder.HeartsAllowed)
                    {
                        Inform("Pierwsze wyłożenie serca!");
                        heartsChanged = true;
                        return true;
                    }

                    return true;
                }
            }


            Inform($"Polecenie serwera to: {serverOrder.Request}");
            return false;

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

        public void SendCard()
        {

            if (Stats.SelectedCards.Count > 0 && CheckSelectedCard(Stats.SelectedCards[0]) && serverOrder != null)
            {
                MessageType responseType = serverOrder.Request == MessageType.YouDeal ? MessageType.YouDeal : MessageType.YourTurn;
                Inform($"Karta wysłana: {Stats.SelectedCards[0]}");

                Message toSend = new Message(responseType, null, Stats.SelectedCards[0])
                {
                    HeartsAllowed = heartsChanged
                };

                clientInstance.SendData(toSend);
                Stats.Hand.Remove(Stats.SelectedCards[0]);
                CardsInHand = new ObservableCollection<Card>(Stats.Hand);
            }
            else
                Inform("Zła karta!");
        }

        public void SendRequestedCard(Card requested)
        {
            for (int i = 0; i < Stats.Hand.Count; i++)
            {
                if (Stats.Hand[i].Equals(requested))
                {
                    Inform($"Posiadałeś: {requested.Name} więc zacząłeś grę!");
                    clientInstance.SendData(new Message(MessageType.CardRequest, null, Stats.Hand[i]));
                    Stats.Hand.RemoveAt(i);
                    CardsInHand = new ObservableCollection<Card>(Stats.Hand);
                    return;
                }

            }

            clientInstance.SendData(new Message(MessageType.CardRequest, null, null));
        }


        //TODO refactor
        private bool CanBePassed()
        {
            if (!IsInDesignMode)
            {
                if (serverOrder != null)
                {
                    if (serverOrder.Request == MessageType.PassOn)
                        return true;
                    else
                        return false;
                }

                return false;
                
            }
            else
                return false;
            
        }

        //TODO check equality comparison
        //TODO change PassOrSelect
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
            List<string> msgs = Messages.ToList();
            msgs.Add(informMessage);
            Messages = new ObservableCollection<string>(msgs);
        }
        
    }
}
