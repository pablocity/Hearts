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
        public ObservableCollection<string> messages;
        public RelayCommand startClick;

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
            server = new Server();
            messages = new ObservableCollection<string>();
            startClick = new RelayCommand(startServer);


        }

        private void startServer()
        {
            if (server == null)
                server = new Server();

            server.StartServerAsync(IPAdress, PortNumber);
        }
    }
}
