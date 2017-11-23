using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MessengerServer
{
    class Client
    {
        IPAddress ip_address;
        TcpClient client;

        public Client(IPAddress ip_address)
        {
            this.ip_address = ip_address;
            this.client = new TcpClient();
        }
    }
}

//We are testing if gitignore works