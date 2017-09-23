using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Hearts.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LibUtility;

namespace Hearts.Server
{
    public class ClientHandler
    {
        public Player PlayerStats { get; set; }
        public string Name { get; private set; }

        private readonly TcpClient client;
        private Message currentResponse = null;

        public ClientHandler(TcpClient client, string name)
        {
            this.client = client;
            Name = name;
            PlayerStats = new Player(Name);
            //TEST PURPOSE DOWN HERE
            //CAN BE OPTIMISED - params doesn't have to be cards array, suits and values are sufficient

            SendSth();

            //Task.Run(async () =>
            //{

            //    while (true)
            //    {

            //        string msg = await SocketUtility.ReceiveData(client);

            //        msg = msg.Replace("\0", String.Empty);

            //        if (!String.IsNullOrWhiteSpace(msg) && !String.IsNullOrEmpty(msg))
            //        {
            //            try
            //            {
            //                JToken.Parse(msg); // if string isn't JSON throws an exception
            //                currentResponse = JsonConvert.DeserializeObject<Message>(msg);

            //                //endFlag = (currentResponse.Request == messageObject.Request); //Checks if response message type is the same as it was in sent data
            //            }
            //            catch (Exception ex)
            //            {
            //                NotifyClient($"Wrong response, it's not a valid message type!\n{ex.Message}");
            //            }
            //        }

            //        msg = "";
            //    }

            //});

        }

        public async Task<Message> SendSth()
        {
            /*Message clientResponse = */return await SendData(new Message(MessageType.CardRequest, new Card(Suits.Clubs, Values.Ace)));
        }

        public async Task<Message> SendData(Message messageObject)
        {

            return await Task.Run(async () =>
            {
                bool endFlag = false;
                Message response = null;
                string clientMsg;
                string toSend = JsonConvert.SerializeObject(messageObject);
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(toSend);

                NetworkStream networkStream = client.GetStream();
                networkStream.Write(bytes.ToArray(), 0, bytes.Length);

                while (!endFlag)
                {

                    byte[] stringResponse = new byte[10240];

                    await networkStream.ReadAsync(stringResponse, 0, (int)client.ReceiveBufferSize);

                    clientMsg = System.Text.Encoding.ASCII.GetString(stringResponse);
                    clientMsg = clientMsg.Replace("\0", String.Empty);

                    if (!String.IsNullOrWhiteSpace(clientMsg) && !String.IsNullOrEmpty(clientMsg))
                    {
                        try
                        {
                            JToken.Parse(clientMsg); // if string isn't JSON throws an exception
                            response = JsonConvert.DeserializeObject<Message>(clientMsg);

                            endFlag = (response.Request == messageObject.Request); //Checks if response message type is the same as it was in sent data
                        }
                        catch (Exception ex)
                        {
                            NotifyClient($"Wrong response, it's not a valid message type!\n{ex.Message}");
                        }
                    }
                }

                return response;
            });

        }

        public void NotifyClient(string error)
        {
            throw new NotImplementedException();
        }
    }
}
