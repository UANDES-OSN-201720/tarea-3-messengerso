using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSO
{
    class Room
    {
        List<Contact> members;
        int id;

        public Room(List<Contact> members, int id)
        {
            this.members = members;
            this.id = id;
        }

        public void Send_message(string message)
        {
            foreach(Contact member in members)
            {
                member.Send_message(message, 'g', id);
            }
        }
    }
}
