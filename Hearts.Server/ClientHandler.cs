using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Hearts.Model;

namespace Hearts.Server
{
    public class ClientHandler
    {
        public string Name { get; private set; }

        TcpClient client;

        public ClientHandler(TcpClient client, string name)
        {
            this.client = client;
            Name = name;
        }
    }
}
