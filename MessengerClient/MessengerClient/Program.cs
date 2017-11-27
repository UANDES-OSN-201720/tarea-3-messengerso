﻿using System;
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
        private static string command;
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
            Console.Title = "Whatsupp";
            Console.WriteLine("List of commands, and what you can do with them:");
            Console.WriteLine("/setname [username] : sets your username \n/tg : talk to group\n" +
                "/t [user] : Talk to specific user, if it exists\n/exit : exit current chat");
            Console.WriteLine("Welcome, " + username);
            send.Start();
            listen.Start();
            Console.ReadLine();
        }
        
        private static void Setname(StreamReader reader)
        {
            if (reader.ToString().ToLower() == "accept")
            {
                username = command.Remove(0, 8);
            }
            else Console.WriteLine("Could not change username: Request denied");
        }
        private static void Group(StreamReader reader)
        {
            if (reader.ToString().ToLower() == "accept")
            {
                Console.WriteLine("You will be connected to public lobby, please wait");
                Thread.Sleep(3000);
                Console.Clear();
                Console.Title = "Public Lobby";
            }
            else Console.WriteLine("Could not connect to lobby: Request denied");
        }
        private static void Priv(StreamReader reader)
        {
            if (reader.ToString().ToLower() == "accept")
            {
                Console.WriteLine("You will be connected to " + command.Remove(0,2) + ", please wait");
                Thread.Sleep(3000);
                Console.Clear();
                Console.Title = command.Remove(0, 2);
            }
            else Console.WriteLine("Could not connect to contact: Request denied");
        }
        private static void Exit(StreamReader reader)
        {
            if (reader.ToString().ToLower() == "accept")
            {
                Console.WriteLine("Exiting chat Window");
                Thread.Sleep(3000);
                Console.Clear();
                Console.Title = "Whatsupp";
                Console.WriteLine("List of commands, and what you can do with them:");
                Console.WriteLine("/setname [username] : sets your username \n/tg : talk to group\n" +
                    "/t [user] : Talk to specific user, if it exists\n/exit : exit current chat");
                Console.WriteLine("Welcome, " + username);
            }
            else Console.WriteLine("Could not exit: not connected to any lobby");
        }
        private static void ConToPriv(StreamReader reader)
        {
            Console.WriteLine("You will be connected to " + reader.ToString().Remove(0, 10));
            Thread.Sleep(3000);
            Console.Clear();
            Console.Title = reader.ToString().Remove(0, 10);

        }

        private static void SendLoop()
        {
            while (true)
            {
                Console.Write("[" + username + "]: ");
                StreamWriter writer = new StreamWriter(ns);
                command = Console.ReadLine();
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
                    if (ReferenceEquals(command.Substring(0, 7), "/setname")) Setname(reader);
                    else if (ReferenceEquals(command, "/tg")) Group(reader);
                    else if (ReferenceEquals(command.Substring(0, 1), "/t")) Priv(reader);
                    else if (ReferenceEquals(command, "/exit")) Exit(reader);
                    else if (ReferenceEquals(reader.ToString().ToLower().Substring(0, 10), "in private:")) ConToPriv(reader);
                    else Console.WriteLine(reader.ReadLine());
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
