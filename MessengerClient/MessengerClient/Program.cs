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
        private static TcpClient tcpClient = new TcpClient();
        private static NetworkStream ns = tcpClient.GetStream();
        private static StreamReader reader = new StreamReader(ns);
        private static string username = reader.ReadLine();
        static void Main(string[] args)
        {
            LoopConnect();
            Console.WriteLine("List of commands, and what you can do with them:");
            Console.WriteLine("/setname : sets your username \n/tg : talk to group\n/" +
                "t [user] : Talk to specific user, if it exists" + "/exit : exit current chat");
            SendLoop();
            Console.ReadLine();
        }

        private static void SendLoop()
        {
            while (true)
            {
                Console.Write("[" + username + "]: ");
                StreamWriter writer = new StreamWriter(ns);
                string command = Console.ReadLine();
                writer.WriteLine(command);
                writer.Flush();
                Console.WriteLine(reader.ReadLine());
            }
        }

        private static void LoopConnect()
        {
            int attempts = 0;
            while (! tcpClient.Connected)
            {
                try
                {
                    attempts++;
                    tcpClient.Connect("192.168.0.178", 1260);
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
