using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer
{
    class Command
    {
        private CommandType type; //Type of command to send
        private IPAddress senderIP;
        private string senderName;
        private IPAddress target;
        private string body;

        public Command(CommandType type, IPAddress target, string metaData)
        {
            this.type = type;
            this.target = target;
            this.body = metaData;

        }
    }
}
