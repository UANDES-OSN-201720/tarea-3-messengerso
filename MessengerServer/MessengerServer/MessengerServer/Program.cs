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
        private static int c_number = 1;
        private static byte[] buffer = new byte[1024];
        private static List<Client> clients = new List<Client>();
        private static TcpListener listener = new TcpListener(IPAddress.Parse("192.168.0.178") , 1260);

        static void Main(string[] args)
        {
            SetupServer();
            Console.Title = "Server";

            Thread accept_thread = new Thread(new ThreadStart(LoopAcceptCallback));
            accept_thread.Start();
            Console.WriteLine("Recibiendo conecciones...");
            
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            listener.Start();
            Console.WriteLine("Server set up!");
        }

        private static void LoopAcceptCallback()
        {
            while (true)
            {
                TcpClient tc = listener.AcceptTcpClient();
                InitializeClient(tc);
                Console.WriteLine("Client connected!");

                NetworkStream ns = tc.GetStream();
                StreamWriter writer = new StreamWriter(ns);
                writer.WriteLine("Guest_" + c_number.ToString());
                writer.Flush();
                c_number++;
            }
        }

        private static void InitializeClient(TcpClient tc)
        {
            Client client = new Client(tc);
            clients.Add(client);
            client.listen = new Thread(new ParameterizedThreadStart(LoopReceiveCallback));
            client.listen.Start(tc);
        }

        private static void LoopReceiveCallback(object tc)
        {
            TcpClient tcl = (TcpClient)tc;
            NetworkStream ns = tcl.GetStream();
            StreamReader reader = new StreamReader(ns);
            while (true)
            {
                try
                {
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
                catch (Exception)
                {
                }
            }
        }
    }
}
