using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GalaSoft.MvvmLight.Messaging;

namespace Hearts.Model
{
    //TODO ustawić ikony dla aplikacji
    public class Client
    {
        private TcpClient client;

        public Client()
        {
            client = new TcpClient();
            
        }

        public async Task ConnectAsync(string ip, int port)
        {
            try
            {
                await client.ConnectAsync(IPAddress.Parse(ip), port);

                Listen();

            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }

        }

        private async void Listen()
        {
            await Task.Run(async () =>
            {
                Message response = null;

                while (true)
                {

                    string message = await ReceiveData(client);


                    if (!String.IsNullOrWhiteSpace(message) && !String.IsNullOrEmpty(message))
                    {
                        ReadMessage(message);

                        if (response == null)
                            continue;

                        if (client.Connected)
                        {
                            NetworkStream ns = client.GetStream();
                            message = JsonConvert.SerializeObject(response);
                            byte[] byteMsg = new byte[15240];
                            byteMsg = System.Text.Encoding.ASCII.GetBytes(message);

                            ns.Write(byteMsg.ToArray(), 0, byteMsg.Length);
                            ns.Flush();
                        }

                    }

                    message = "";
                }

            });


        }

        private async Task<string> ReceiveData(TcpClient client)
        {
            if (client.Connected)
            {
                NetworkStream data = client.GetStream();

                byte[] byteMsg = new byte[15240];

                int i = await data.ReadAsync(byteMsg, 0, (int)client.ReceiveBufferSize);

                string message = System.Text.Encoding.ASCII.GetString(byteMsg);

                message = message.Replace("\0", String.Empty);

                //data.Flush();


                return message;
            }
            else
                return "";
            
        }

        private void ReadMessage(string JSON_Message)
        {
            try
            {

                JToken.Parse(JSON_Message); //If not valid JSON throw exception

                Message serverRequest = JsonConvert.DeserializeObject<Message>(JSON_Message);

                Messenger.Default.Send<Message>(serverRequest);

            }
            catch (Exception ex)
            {
                Error(ex.Message);
                return;
            }

        }


        

        public void SendData(Message data)
        {
            string toSend = JsonConvert.SerializeObject(data);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(toSend);

            NetworkStream networkStream = client.GetStream();
            networkStream.Write(bytes.ToArray(), 0, bytes.Length);
            networkStream.Flush();
        }

        private void Error(string message)
        {
            Console.WriteLine(message);
            //throw new NotImplementedException();
        }
    }
}
