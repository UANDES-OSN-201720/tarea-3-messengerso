using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace MessengerServer
{
    class Program
    {
        private static byte[] buffer = new byte[1024];
        private static List<TcpClient> client_sockets = new List<TcpClient>();
        private static TcpListener listener = new TcpListener(IPAddress.Parse("192.168.0.178") , 1260);
        private static TcpClient t_client;

        static void Main(string[] args)
        {
            SetupServer();
            Console.Title = "Server";

            Thread accept_thread = new Thread(new ThreadStart(LoopAcceptCallback));
            accept_thread.Start();
            Console.WriteLine("Recibiendo conecciones...");
            
            ReciveCallback(t_client);
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            listener.Start();
        }

        private static void LoopAcceptCallback()
        {
            while (true)
            {
                TcpClient tc = listener.AcceptTcpClient();
                t_client = tc;
                Console.WriteLine("Client connected!");
            }
        }

        private static void ReciveCallback(TcpClient tc)
        {
            NetworkStream ns = tc.GetStream();
            StreamReader reader = new StreamReader(ns);

            string message_client = reader.ReadLine();
            Console.WriteLine("Client says: " + message_client);

            string response;
            if (message_client.ToLower() == "get time")
            {
                response = DateTime.Now.ToLongTimeString();
            }
            else
            {
                response = "Invalid request";
            }

            StreamWriter writer = new StreamWriter(ns);
            writer.WriteLine(response);
            writer.Flush();
        }
    }
}
