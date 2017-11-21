using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSO
{
    class Contact
    {
        public string name;
        public int IP;

        public Contact(string name, int IP)
        {
            this.name = name;
            this.IP = IP;
        }

        public void Send_message(string message, char flag, int group_id = -1)
        {
            // The message is sent with the corresponding flag
        }
    }
}
