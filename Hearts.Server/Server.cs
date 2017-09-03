
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Hearts.Model;

namespace Hearts.Server
{
    public class Server
    {
        TcpListener listener;

        List<ClientHandler> clients;

        int clientNumber;

        public Server()
        {
            clients = new List<ClientHandler>();

        }

        public async void StartServerAsync(string ipAdress, int portNumber)
        {

            try
            {
                if (IPAddress.TryParse(ipAdress, out IPAddress ip))
                    listener = new TcpListener(ip, portNumber);
                else
                {
                    Error("Nieprawidłowy adres IP");
                    return;
                }

                await Task.Run(async () =>
                {
                    listener.Start();

                    while (clients.Count < 3)
                    {
                        clientNumber++;

                        TcpClient client = await listener.AcceptTcpClientAsync();

                        ClientHandler handler = new ClientHandler(client, String.Format("Gracz nr. {0}", clientNumber));

                        clients.Add(handler);
                    }
                });

            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

        }

        public void StartGameAsync()
        {

        }

        private void Error(string errorMessage)
        {

        }
    }
}
