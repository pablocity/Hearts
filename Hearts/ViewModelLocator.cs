using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearts.ViewModels;

namespace Hearts
{
    public class ViewModelLocator
    {
        public static ClientViewModel clientViewModel { get; private set; } = new ClientViewModel();
        public static LobbyViewModel lobbyViewModel { get; private set; } = new LobbyViewModel();
    }
}
