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
        public static GameViewModel clientViewModel { get; private set; } = new GameViewModel();
        public static LobbyViewModel lobbyViewModel { get; private set; } = new LobbyViewModel();
    }
}
