using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Hearts.Model;
using System.Collections.ObjectModel;
using System.Timers;

namespace Hearts.ViewModels
{
    public class GameViewModel : ViewModelBase
    {

        public Player Stats;

        private Message serverOrder = null;

        private bool heartsChanged = false;

        private Timer potTimer;


        private int points;
        public int Points
        {
            get
            {
                return points;
            }
            private set
            {
                Set<int>(() => Points, ref points, value);
            }
        }

        private Client clientInstance;
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

        private ObservableCollection<Card> pot;
        public ObservableCollection<Card> Pot
        {
            get
            {
                return pot;
            }
            private set
            {
                pot = value;
                RaisePropertyChanged(() => Pot);
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


        private string helpInfo;
        public string HelpInfo
        {
            get
            {
                return helpInfo;
            }
            set
            {
                Set<string>(() => HelpInfo, ref helpInfo, value);
            }
        }

        public RelayCommand passCards { get; private set; }
        public RelayCommand sendCard { get; private set; }
        public RelayCommand<Card> selectCard { get; private set; }

        public GameViewModel()
        {
            potTimer = new Timer()
            {
                Interval = 2000
            };

            potTimer.Elapsed += (s, e) =>
            {
                
                //Pot = new ObservableCollection<Card>();
                potTimer.Stop();
            };


            HelpInfo = "Here you will see a game guide";
            Stats = new Player("");
            Pot = new ObservableCollection<Card>();
            Messages = new ObservableCollection<string>();
            cardsInHand = new ObservableCollection<Card>();

            passCards = new RelayCommand(PassOn, () =>
            {
                if (serverOrder != null && serverOrder.Request == MessageType.PassOn)
                    return true;
                else
                    return false;
            });

            sendCard = new RelayCommand(SendCard, () =>
            {
                if (serverOrder != null && (serverOrder.Request == MessageType.YourTurn || serverOrder.Request == MessageType.YouDeal))
                    return true;
                else
                    return false;
            });


            selectCard = new RelayCommand<Card>(Select);

            Messenger.Default.Register<Message>(this, RetrieveMessage);
            
        }

        private void RetrieveMessage(Message serverRequest)
        {
            
            //Messages.Add("Client configured");
            serverOrder = serverRequest;
            
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                passCards.RaiseCanExecuteChanged();
                sendCard.RaiseCanExecuteChanged();
            });

            switch (serverRequest.Request)
            {
                case MessageType.CardRequest:
                    Inform($"Server requested: {serverOrder.CardsRequested[0]}", true);
                    SendRequestedCard(serverOrder.CardsRequested[0]);
                    break;
                case MessageType.ShowCards:
                    ShowCards(serverRequest);
                    break;
                case MessageType.PassOn:
                    Inform("Now is your turn to pass on 3 cards");
                    break;
                case MessageType.ShowPot:
                    Inform($"Pot contains: {serverOrder.CardsRequested}", true);
                    ShowPot(serverOrder.CardsRequested);
                    Points = serverOrder.PlayerStats.Points;
                    //Inform($"\nPunkty: {serverOrder.PlayerStats.Points}\n");
                    break;
                case MessageType.YouDeal:
                    Inform($"You deal now!");
                    break;
                case MessageType.YourTurn:
                    Inform("Your turn now!");
                    break;
                case MessageType.ShowStats:
                    Points = serverOrder.PlayerStats.Points;
                    //Inform($"\nPunkty: {serverOrder.PlayerStats.Points}\n");
                    break;
                case MessageType.Win:
                    Inform("You won the game!");
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

            //Inform("Odświeżono karty");
            
        }

        public void ShowPot(IEnumerable<Card> pot)
        {
            
            Pot = new ObservableCollection<Card>(pot);
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
                            Inform("You can't put it yet because you have other suits and nobody had played hearts before!");
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
                    Inform("You need to choose 3 cards!");
                }
            }
        }

        public void SendCard()
        {

            if (Stats.SelectedCards.Count > 0 && CheckSelectedCard(Stats.SelectedCards[0]) && serverOrder != null)
            {
                MessageType responseType = serverOrder.Request == MessageType.YouDeal ? MessageType.YouDeal : MessageType.YourTurn;
                Inform($"Sent card: {Stats.SelectedCards[0].Name}", true);
                Stats.Hand.Remove(Stats.SelectedCards[0]);
                //TODO Check if sending playerStats doesn't spoil anything
                Message toSend = new Message(responseType, Stats, Stats.SelectedCards[0])
                {
                    HeartsAllowed = heartsChanged
                };

                clientInstance.SendData(toSend);
                Stats.Hand.Remove(Stats.SelectedCards[0]);
                CardsInHand = new ObservableCollection<Card>(Stats.Hand);
                Stats.SelectedCards.Clear();
            }
            else
                Inform("Wrong card!");
        }

        public void SendRequestedCard(Card requested)
        {
            for (int i = 0; i < Stats.Hand.Count; i++)
            {
                if (Stats.Hand[i].Equals(requested))
                {
                    Inform($"You owned: {requested.Name} so you started the game!");
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
            //Inform("Select");
            if (Stats.PassOrSelect)
            {
                if (Stats.SelectedCards.Contains(toSelect))
                {
                    Stats.SelectedCards.Remove(toSelect);
                    Inform($"{toSelect.Name} was removed");
                }
                else if (!Stats.SelectedCards.Contains(toSelect) && Stats.SelectedCards.Count < 3)
                {
                    Stats.SelectedCards.Add(toSelect);
                    Inform($"{toSelect.Name} was added");
                }
                else
                {
                    Card lastOne = Stats.SelectedCards[Stats.SelectedCards.Count - 1];
                    Stats.SelectedCards[Stats.SelectedCards.Count - 1] = toSelect;
                    Inform($"{lastOne.Name} was replaced by {toSelect.Name}");
                }

            }
            else
            {
                Stats.SelectedCards.Clear();
                Stats.SelectedCards.Add(toSelect);
            }
        }


        private void Inform(string informMessage, bool isServerMsg = false)
        {
            Console.WriteLine(informMessage);

            if (isServerMsg)
            {
                List<string> msgs = Messages.ToList();
                msgs.Add(informMessage);
                Messages = new ObservableCollection<string>(msgs);
            }
            else
                HelpInfo = informMessage;

        }
        
    }
}
