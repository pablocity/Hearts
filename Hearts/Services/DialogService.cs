using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Hearts.Views;
using Hearts.ViewModels;
using Hearts.Model;

namespace Hearts.Services
{
    public class DialogService
    {
        static Window window = null;
        public static void OpenDialog(Client client)
        {
            window = new GameView();
            GameViewModel viewModel = window.DataContext as GameViewModel;
            viewModel.ClientInstance = client;
            window.ShowDialog();
        }

        public static void CloseDialog()
        {
            if (window != null)
                window.Close();
        }
    }
}
