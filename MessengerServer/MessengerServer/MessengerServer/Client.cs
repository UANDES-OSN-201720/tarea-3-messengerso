using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace MessengerServer
{
    class Client
    {
        public Thread listen;
        TcpClient client;

        public Client(TcpClient tc)
        {
            this.client = tc;
        }
    }
}

//We are testing if gitignore works