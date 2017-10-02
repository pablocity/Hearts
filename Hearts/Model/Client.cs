using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Hearts.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace Hearts.Model
{
    //TODO ustawić ikony dla aplikacji
    public class Client
    {
        TcpClient client;

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
                throw new NotImplementedException();
            }

        }

        private async void Listen()
        {
            await Task.Run(async () =>
            {
                Message response = null;

                while (true)
                {

                    string msg = await ReceiveData(client);

                    //msg = msg.Replace("\0", String.Empty);


                    if (!String.IsNullOrWhiteSpace(msg) && !String.IsNullOrEmpty(msg))
                    {
                        response = await ReadMessage(msg);

                        if (response == null)
                            continue;

                        if (client.Connected)
                        {
                            NetworkStream ns = client.GetStream();
                            msg = JsonConvert.SerializeObject(response);
                            byte[] byteMsg = new byte[10240];
                            byteMsg = System.Text.Encoding.ASCII.GetBytes(msg);

                            ns.Write(byteMsg.ToArray(), 0, byteMsg.Length);
                            ns.Flush();
                        }


                        //TODO flush stream everywhere after closing app
                    }

                    msg = "";
                }

            });


        }

        private async Task<string> ReceiveData(TcpClient client)
        {
            if (client.Connected)
            {
                NetworkStream data = client.GetStream();

                byte[] byteMsg = new byte[10240];

                int i = await data.ReadAsync(byteMsg, 0, (int)client.ReceiveBufferSize);

                string message = System.Text.Encoding.ASCII.GetString(byteMsg);

                message = message.Replace("\0", String.Empty);

                //data.Flush();


                return message;
            }
            else
                return "";
            
        }

        private async Task<Message> ReadMessage(string JSON_Message)
        {
            try
            {
                Message response = null;

                //JToken.Parse(JSON_Message); //If not JSON throw exception

                Message serverRequest = JsonConvert.DeserializeObject<Message>(JSON_Message);

                switch (serverRequest.Request)
                {
                    case MessageType.CardRequest:
                        Messenger.Default.Send<Message>(serverRequest);
                        return await ViewModelLocator.clientViewModel.SelectCard(serverRequest);
                    case MessageType.ShowCards:
                        Messenger.Default.Send<Message>(serverRequest);
                        return null;
                    case MessageType.PassOn:
                        return await ViewModelLocator.clientViewModel.PassOn();
                    default:
                        Error("Wrong suited case!");
                        break;
                }

                return response;
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                return null;
            }

        }


        

        private void SendData()
        {

        }

        private void Error(string message)
        {
            Console.WriteLine(message);
            //throw new NotImplementedException();
        }
    }
}
