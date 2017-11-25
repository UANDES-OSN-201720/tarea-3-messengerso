using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace MessengerClient
{
    class Program
    {
        private static TcpClient tcpClient = new TcpClient();
        private static NetworkStream ns;
        private static StreamReader reader;
        private static string username;
        static void Main(string[] args)
        {
            Thread listen = new Thread(new ThreadStart(ListenLoop));
            Thread send = new Thread(new ThreadStart(SendLoop));
            LoopConnect();
            ns = tcpClient.GetStream();
            try
            {
                reader = new StreamReader(ns);
                username = reader.ReadLine();
            }
            catch { }
            Console.WriteLine("List of commands, and what you can do with them:");
            Console.WriteLine("/setname [username] : sets your username \n/tg : talk to group\n" +
                "/t [user] : Talk to specific user, if it exists\n/exit : exit current chat");
            Console.WriteLine("Welcome, " + username);
            send.Start();
            listen.Start();
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
            }
        }
        private static void ListenLoop()
        {
            while (true)
            {
                try
                {
                    reader = new StreamReader(ns);
                    Console.WriteLine(reader.ReadLine());
                }
                catch { }
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
