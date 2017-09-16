using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Hearts.Views;

namespace Hearts.Services
{
    public class DialogService
    {
        static Window window = null;
        public static void OpenDialog()
        {
            window = new GameView();
            window.ShowDialog();
        }

        public static void CloseDialog()
        {
            if (window != null)
                window.Close();
        }
    }
}
