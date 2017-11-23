using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MessengerServer
{
    class Program
    {
        private static byte[] buffer = new byte[1024];
        private static List<Socket> client_sockets = new List<Socket>();
        private static Socket server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            SetupServer();
            Console.Title = "Whatsupp";
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            server_socket.Bind(new IPEndPoint(IPAddress.Any, 8000));
            server_socket.Listen(10);
            server_socket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = server_socket.EndAccept(AR);
            client_sockets.Add(socket);
            Console.WriteLine("Client connected!");
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), socket);
            server_socket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReciveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int recived = socket.EndReceive(AR);
            byte[] data_buff = new byte[recived];
            Array.Copy(buffer, data_buff, recived);

            string text = Encoding.UTF8.GetString(data_buff);
            Console.WriteLine("Text recived: " + text);

            string response;
            if (text.ToLower() == "get time")
            {
                response = DateTime.Now.ToLongTimeString();
            }
            else
            {
                response = "Invalid request";
            }

            byte[] data = Encoding.UTF8.GetBytes(response);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), socket);
        }

        private void SendMessage(string text)
        {
        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}
