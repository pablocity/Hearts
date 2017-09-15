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

namespace Hearts.Server
{
    public class ClientHandler
    {
        public string Name { get; private set; }

        readonly TcpClient client;


        public ClientHandler(TcpClient client, string name)
        {
            this.client = client;
            Name = name;

            //TEST PURPOSE DOWN HERE
            //CAN BE OPTIMISED - params doesn't have to be cards array, suits and values are sufficient
            SendData(new Message(MessageType.CardRequest, new Card(Suits.Clubs, Values.Ace)));
        }

        public async void SendData(Message messageObject)
        {

            await Task.Run(async () =>
            {
                bool endFlag = false;
                Message response;
                string clientMsg;
                string toSend = JsonConvert.SerializeObject(messageObject);
                List<byte> bytes = System.Text.Encoding.ASCII.GetBytes(toSend).ToList();

                using (NetworkStream networkStream = client.GetStream())
                {
                    networkStream.Write(bytes.ToArray(), 0, bytes.Count);

                    while (!endFlag)
                    {

                        List<byte> stringResponse = new List<byte>();
                        // TODO sprawdzic czy dziala zamiana na tablice
                        await networkStream.ReadAsync(stringResponse.ToArray(), 0, (int) client.ReceiveBufferSize);

                        clientMsg = System.Text.Encoding.ASCII.GetString(stringResponse.ToArray());
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

                    //process response message
                }
            });

        }

        public void NotifyClient(string error)
        {
            throw new NotImplementedException();
        }
    }
}
