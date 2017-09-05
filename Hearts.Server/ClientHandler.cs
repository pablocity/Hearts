﻿using System;
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
                                JToken.Parse(clientMsg);
                                response = JsonConvert.DeserializeObject<Message>(clientMsg);

                                endFlag = (response.Request == messageObject.Request);
                            }
                            catch (Exception ex)
                            {
                                NotifyClient($"Wrong response, it's not a valid message type!\n{ex.Message}");
                            }
                        }
                    }
                }
            });

        }

        public void NotifyClient(string error)
        {
            
        }
    }
}