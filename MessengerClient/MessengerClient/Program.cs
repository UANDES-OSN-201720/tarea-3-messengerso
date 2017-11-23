using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MessengerClient
{
    class Program
    {
        private static Socket client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            LoopConnect();
            SendLoop();
            Console.ReadLine();
        }

        private static void SendLoop()
        {
            while (true)
            {
                Console.Write("Enter a request: ");
                string request = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(request);
                client_socket.Send(buffer);

                byte[] received_buff = new byte[1024];
                int received = client_socket.Receive(received_buff);
                byte[] data = new byte[received];
                Array.Copy(received_buff, data, received);
                Console.WriteLine("Received: " + Encoding.UTF8.GetString(data));
            }
        }

        private static void LoopConnect()
        {
            int attempts = 0;
            while (! client_socket.Connected)
            {
                try
                {
                    attempts++;
                    client_socket.Connect(IPAddress.Loopback, 8000);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Connection attempts: " + attempts.ToString());
                }
            }
            Console.Clear();
            Console.WriteLine("Connected!");
        }
    }
}
