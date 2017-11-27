﻿using System;
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
        private static List<Client> group_chat = new List<Client>();
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
                string username = "Guest_" + c_number.ToString();
                c_number++;
                InitializeClient(tc, username);
                Console.WriteLine(username + " connected!");

                NetworkStream ns = tc.GetStream();
                StreamWriter writer = new StreamWriter(ns);
                writer.WriteLine(username);
                writer.Flush();
            }
        }

        private static void InitializeClient(TcpClient tc, string username)
        {
            Client client = new Client(tc, username);
            clients.Add(client);
            client.listen = new Thread(new ParameterizedThreadStart(LoopReceiveCallback));
            client.listen.Start(client);
        }

        private static void LoopReceiveCallback(object client)
        {
            Client tcp_client = (Client)client;
            NetworkStream ns = tcp_client.client.GetStream();
            StreamReader reader = new StreamReader(ns);
            while (true)
            {
                try
                {
                    string message_client = reader.ReadLine();
                    Console.WriteLine("[" + tcp_client.username + "]" + message_client);

                    string response;
                    string first_word = message_client.ToLower().Substring(0, message_client.IndexOf(" "));
                    if (first_word.ToLower() == "/tg" && !tcp_client.in_group && !tcp_client.in_private)
                    {
                        tcp_client.in_group = true;
                        group_chat.Add(tcp_client);
                        response = "Accept";
                    }
                    else if (first_word.ToLower() == "/t" && !tcp_client.in_group && !tcp_client.in_private)
                    {
                        string second_word = message_client.Substring(message_client.IndexOf(" "), -1);
                        Client listener_client = FindClientByUsername(second_word);
                        if (listener_client is null) response = "Decline";
                        else
                        {
                            if (!listener_client.in_private && !listener_client.in_group)
                            {
                                listener_client.SendMessage("in private:" + tcp_client.username);
                                listener_client.in_private = true;
                                listener_client.private_username = tcp_client.username;
                                tcp_client.private_username = listener_client.username;
                                response = "Accept";
                            }
                            else response = "Decline";
                        }
                    }
                    else if (first_word.ToLower() == "/exit" && tcp_client.in_group)
                    {
                        tcp_client.in_group = false;
                        group_chat.Remove(tcp_client);
                        response = "Accept";
                    }
                    else if(first_word.ToLower() == "/exit" && tcp_client.in_private)
                    {
                        tcp_client.in_private = false;
                        Client listener_client = FindClientByUsername(tcp_client.private_username);
                        listener_client.private_username = "";
                        tcp_client.private_username = "";
                        response = "Accept";
                    }
                    else if (first_word.ToLower() == "/setname")
                    {
                        bool found_space = false;
                        string second_word = message_client.Substring(message_client.IndexOf(" "), -1);
                        foreach(char character in second_word)
                        {
                            if (character == ' ') found_space = true;
                        }
                        bool username_exists = false;
                        foreach(Client person in clients)
                        {
                            if (person.username == second_word) username_exists = true;
                        }
                        response = "Decline";
                        if (!found_space && !username_exists)
                        {
                            tcp_client.username = second_word;
                            response = "Accept";
                        }
                    }
                    else if (tcp_client.in_private)
                    {
                        Client listener_client = FindClientByUsername(tcp_client.private_username);
                        listener_client.SendMessage("["+tcp_client.username+"]: "+message_client);
                        response = "Accept";
                    }
                    else if (tcp_client.in_group)
                    {
                        SendMessageToGroup("[" + tcp_client.username + "]: " + message_client, tcp_client);
                        response = "Accept";
                    }
                    else
                    {
                        response = "Invalid request";
                    }

                    StreamWriter writer = new StreamWriter(ns);
                    writer.WriteLine(response);
                    writer.Flush();
                }
                catch (EndOfStreamException)
                {
                    Console.WriteLine("EOS");
                }
                catch (Exception)
                {
                }
            }
        }

        private static Client FindClientByUsername(string username)
        {
            foreach (Client client in clients)
            {
                if (client.username == username) return client;
            }
            return null;
        }

        private static void SendMessageToGroup(string message, Client sender)
        {
            foreach(Client client in group_chat)
            {
                if(!(client.username == sender.username)) client.SendMessage(message);
            }
        }
    }
}
