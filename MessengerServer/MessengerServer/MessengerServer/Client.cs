using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MessengerServer
{
    class Client
    {
        public string username;
        public Thread listen;
        public TcpClient client;
        public bool in_group, in_private;
        public string private_username;

        public Client(TcpClient tc, string username)
        {
            this.username = username;
            this.client = tc;
            this.in_group = false;
            this.in_private = false;
            this.private_username = "";
        }

        public void SendMessage(string message)
        {
            NetworkStream ns = this.client.GetStream();
            StreamWriter writer = new StreamWriter(ns);
            writer.WriteLine(message);
            writer.Flush();
        }
    }
}

//We are testing if gitignore works