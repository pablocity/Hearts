using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearts.Server.ViewModels;

namespace Hearts.Server
{
    public class ViewModelLocator
    {
        public static MainViewViewModel mainViewModel { get; private set; } = new MainViewViewModel();
    }
}
