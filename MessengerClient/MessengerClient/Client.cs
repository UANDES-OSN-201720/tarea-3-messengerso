using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;

namespace MessengerClient
{
    [Serializable]
    class Client
    {
        private string name;
        private List<Contact> contacts;
        public IPAddress IP;
        private TcpClient client;
        [NonSerialized] private string state;
        [NonSerialized] private Room active_room;
        [NonSerialized] private int active_chat_IP;

        public Client(string name, IPAddress IP)
        {
            this.name = name;
            this.contacts = null;
            this.IP = IP;
            this.client = new TcpClient();
        }

        private int Get_last_id()
        {
            int last_id = -1;
            foreach (Contact contact in contacts)
            {
                if (contact.IP > last_id) last_id = contact.IP;
            }
            return last_id;
        }
        public void Create_contact(string name, int IP)
        {
            contacts.Add(new Contact(name, IP));
        }

        public void Post_state()
        {
            switch (state)
            {
                case "connected":
                    foreach (Contact contact in contacts)
                    {
                        //Send an "I'm connected" message
                    }
                    break;
                case "disconnected":
                    foreach (Contact contact in contacts)
                    {
                        //Send an "I'm disconnected" message
                    }
                    break;
                case "busy":
                    foreach (Contact contact in contacts)
                    {
                        //Send an "I'm busy" message
                    }
                    break;
                default:
                    //do nothing
                    break;
            }
        }

        public void Connect(IPAddress server, Socket socket)
        {
            client.Connect(server, 8000);
            state = "connected";
            Post_state();
        }

        public void Desconnect()
        {
            state = "disconnected";
            Post_state();
            //disconect
        }

        public void Call(Contact contact)
        {
            //call contact with return to a boolean variable
            bool Success = true;
            if (Success)
            {
                //set active_chat_IP to call's IP
                state = "busy";
                Post_state();
            }
        }

        public void Create_room(List<Contact> room_contacts)
        {
            foreach (Contact contact in room_contacts)
            {
                //Invite to room
            }
            //set active_room to a new id
            state = "busy";
            Post_state();
        }

        public void Leave_call_or_room()
        {
            if (active_chat_IP != -1)
            {
                //send "I'm leaving message to contact
                active_chat_IP = -1;
            }
            else if (active_room != null)
            {
                //send "I'm leaving message to room
                active_room = null;
            }
        }
    }
}
