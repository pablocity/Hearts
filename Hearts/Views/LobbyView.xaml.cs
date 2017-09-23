using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hearts.Model;
using Hearts.Services;

namespace Hearts.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LobbyView.xaml
    /// </summary>
    public partial class LobbyView : Window
    {
        Client client;
        public LobbyView()
        {
            InitializeComponent();
            client = new Client();
            IP.Text = "127.0.0.1";
            Port.Text = "8080";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await client.ConnectAsync(IP.Text, int.Parse(Port.Text));

            //DialogService.OpenDialog();

            //ViewModelLocator.clientViewModel.ClientInstance = client;

            //this.Close();
            //TODO remove test case
        }
    }
}
