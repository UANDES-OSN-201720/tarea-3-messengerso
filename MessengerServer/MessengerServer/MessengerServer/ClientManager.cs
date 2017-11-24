using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer
{
    public class ClientManager
    {
        private Socket socket;
        private string client_name;

        public IPAddress IP
        {
            get
            {
                if (this.socket != null)
                {
                    return ((IPEndPoint)this.socket.RemoteEndPoint).Address;
                }
                else return IPAddress.None;
            }
        }//Gets IP address. None if not connected
        public int Port
        {
            get
            {
                if (this.socket != null)
                {
                    return ((IPEndPoint)this.socket.RemoteEndPoint).Port;
                }
                else return -1;
            }
        }
        public bool Connected
        {
            get
            {
                if (this.socket != null) return this.socket.Connected;
                else return false;
            }
        }

        public string Client_name { get => client_name; set => client_name = value; }
        NetworkStream networkStream;
        private BackgroundWorker bwReceiver;//Used for multithreading, like for acceptance from the server

        //Client Manager Constructor
        public ClientManager(Socket clientSocket)
        {
            this.socket = clientSocket;
            this.networkStream = new NetworkStream(this.socket);
            this.bwReceiver = new BackgroundWorker();
            this.bwReceiver.DoWork += new DoWorkEventHandler(StartReceive);
            this.bwReceiver.RunWorkerAsync();
        }

        private void StartReceive(object sender, DoWorkEventArgs e)
        {
            while (this.socket.Connected)
            {
                //Command type
                byte[] buffer = new byte[1024];
                int readBytes = this.networkStream.Read(buffer, 0, 1024);
                if (readBytes == 0) break;
                CommandType cmd = (CommandType)(BitConverter.ToInt32(buffer, 0));
                //Command's target size
                string target = "";
                buffer = new byte[1024];
                readBytes = this.networkStream.Read(buffer, 0, 4);
                if (readBytes == 0) break;
                int ipSize = BitConverter.ToInt32(buffer, 0);
                //Command's target
                buffer = new byte[ipSize];
                readBytes = this.networkStream.Read(buffer, 0, ipSize);
                if (readBytes == 0) break;
                target = Encoding.UTF8.GetString(buffer);
                //Command's METADATA size
                string MetaData = "";
                buffer = new byte[1024];
                readBytes = this.networkStream.Read(buffer, 0, 1024);
                if (readBytes == 0) break;
                int MetaDataSize = BitConverter.ToInt32(buffer, 0);
                //Command's METADATA
                buffer = new byte[MetaDataSize];
                readBytes = this.networkStream.Read(buffer, 0, MetaDataSize);
                if (readBytes == 0) break;
                MetaData = Encoding.Unicode.GetString(buffer);
                


            }
        }
    }
}
