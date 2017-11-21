using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MessengerServer
{
    class Server
    {
        List<Client> clients;
        Socket socket;
        IPAddress ip_address;
        IPEndPoint ipe;

        public Server(IPAddress ip_address)
        {
            this.ip_address = ip_address;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }
    }
}
