using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Hearts.Model;

namespace Hearts.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        //Game instance
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

        public GameViewModel()
        {

        }
    }
}
