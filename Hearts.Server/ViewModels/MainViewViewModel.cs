using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;

namespace Hearts.Server.ViewModels
{
    public class MainViewViewModel : ViewModelBase
    {

        private ObservableCollection<string> messages;
        public ObservableCollection<string> Messages
        {
            get
            {
                return messages;
            }

            set
            {
                messages = value;
                RaisePropertyChanged(() => Messages);
            }
        }
        public RelayCommand StartClick { get; private set; }

        private string ipAdress;
        public string IPAdress
        {
            get
            {
                return ipAdress;
            }
            set
            {
                Set<string>(() => IPAdress, ref ipAdress, value);
            }
        }

        private int portNumber;
        public int PortNumber
        {
            get
            {
                return portNumber;
            }
            set
            {
                Set<int>(() => PortNumber, ref portNumber, value);
            }
        }

        private Server server;

        public MainViewViewModel()
        {
            IPAdress = "127.0.0.1";
            PortNumber = 8080;
            server = new Server();
            messages = new ObservableCollection<string>();
            StartClick = new RelayCommand(startServer);
        }

        private void startServer()
        {
            if (server == null)
                server = new Server();

            server.StartServerAsync(IPAdress, PortNumber);

            Messages.Add($"Server has started succesfully\n IP: {IPAdress} port number {PortNumber}");
        }
    }
}
