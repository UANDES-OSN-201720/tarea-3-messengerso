using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MessengerClient
{
    class Program
    {
        private static TcpClient client_socket = new TcpClient();

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
                NetworkStream ns = client_socket.GetStream();
                StreamWriter writer = new StreamWriter(ns);
                string request = Console.ReadLine();
                writer.WriteLine(request);
                writer.Flush();

                StreamReader reader = new StreamReader(ns);
                Console.WriteLine("Recieved: " + reader.ReadLine());                
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
                    client_socket.Connect(IPAddress.Any, 1260);
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
