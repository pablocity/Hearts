
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
                    //TODO remove test case
                    while (clients.Count < 2)
                    {
                        clientNumber++;

                        TcpClient client = await listener.AcceptTcpClientAsync();

                        ClientHandler handler = new ClientHandler(client, String.Format("Gracz nr. {0}", clientNumber));

                        clients.Add(handler);
                    }

                    StartGameAsync();

                });

            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

        }

        public void StartGameAsync()
        {
            Game.Instance.Players = clients;
            Game.Instance.StartGame();
            Error("Game started but not implemented!");
        }

        public async Task<List<string>> SendMessage()
        {
            List<string> messages = new List<string>();

            foreach (ClientHandler cl in clients)
            {
                Message m = await cl.SendData(new Message(MessageType.CardRequest, null, new Card(Suits.Clubs, Values.Eight)));
                messages.Add($"{m.CardsRequested[0]}\n{m.Request.ToString()}\n");
            }

            return messages;
        }

        private static void Error(string errorMessage)
        {
            Console.WriteLine(errorMessage);
            //throw new NotImplementedException();
        }
    }
}
