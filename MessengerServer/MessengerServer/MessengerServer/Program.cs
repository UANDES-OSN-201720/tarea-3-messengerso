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
        private static TcpListener listener = new TcpListener(IPAddress.Loopback/*Parse("192.168.50.18")*/ , 1260);

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
            int fails = 0;
            while (true)
            {
                try
                {
                    string message_client = reader.ReadLine();
                    Console.WriteLine("[" + tcp_client.username + "] " + message_client);

                    string response;
                    if (message_client.Length > 0)
                    {
                        string first_word;
                        try
                        {
                            first_word = message_client.ToLower().Substring(0, message_client.IndexOf(" "));
                        }
                        catch (Exception)
                        {
                            first_word = message_client;
                        }

                        if (first_word.ToLower() == "/tg" && !tcp_client.in_group && !tcp_client.in_private)
                        {
                            tcp_client.in_group = true;
                            response = "accept";
                        }
                        else if (first_word.ToLower() == "/t" && !tcp_client.in_group && !tcp_client.in_private)
                        {
                            string second_word = message_client.Remove(0, 3);
                            Client listener_client = FindClientByUsername(second_word);
                            if (listener_client is null) response = "decline";
                            else
                            {
                                if (!listener_client.in_private && !listener_client.in_group)
                                {
                                    listener_client.SendMessage("in private:" + tcp_client.username);
                                    listener_client.in_private = true;
                                    listener_client.private_username = tcp_client.username;
                                    tcp_client.private_username = listener_client.username;
                                    response = "accept";
                                }
                                else response = "decline";
                            }
                        }
                        else if (first_word.ToLower() == "/exit" && tcp_client.in_group)
                        {
                            tcp_client.in_group = false;
                            response = "accept";
                        }
                        else if (first_word.ToLower() == "/exit" && tcp_client.in_private)
                        {
                            tcp_client.in_private = false;
                            Client listener_client = FindClientByUsername(tcp_client.private_username);
                            listener_client.private_username = "";
                            tcp_client.private_username = "";
                            response = "accept";
                        }
                        else if (first_word.ToLower() == "/setname")
                        {
                            bool found_space = false;
                            bool found_name = true;
                            string second_word;
                            try
                            {
                                second_word = message_client.Remove(0, 9);
                            }
                            catch (Exception)
                            {
                                second_word = "";
                                found_name = false;
                            }
                            if (found_name)
                            {
                                foreach (char character in second_word)
                                {
                                    if (character == ' ')
                                    {
                                        found_space = true;
                                    }
                                }
                                bool username_exists = false;
                                foreach (Client person in clients)
                                {
                                    if (person.username == second_word) username_exists = true;
                                }
                                response = "decline";
                                if (!found_space && !username_exists)
                                {
                                    tcp_client.username = second_word;
                                    response = "accept";
                                }
                            }
                            else response = "decline";
                        }
                        else if (tcp_client.in_private)
                        {
                            Client listener_client = FindClientByUsername(tcp_client.private_username);
                            listener_client.SendMessage("[" + tcp_client.username + "]: " + message_client);
                            response = "accept";
                        }
                        else if (tcp_client.in_group)
                        {
                            SendMessageToGroup("[" + tcp_client.username + "]: " + message_client, tcp_client);
                            response = "accept";
                        }
                        else
                        {
                            response = "Invalid request";
                        }
                        fails = 0;

                        StreamWriter writer = new StreamWriter(ns);
                        writer.WriteLine(response);
                        writer.Flush();
                    }
                    else if (message_client.Length < 0)
                    {
                        response = "decline";
                        fails = 0;

                        StreamWriter writer = new StreamWriter(ns);
                        writer.WriteLine(response);
                        writer.Flush();
                    }
                }
                catch (Exception)
                {
                    fails++;
                    if (fails > 3)
                    {
                        clients.Remove(tcp_client);
                        if (tcp_client.in_private)
                        {
                            try
                            {
                                Client private_mate = FindClientByUsername(tcp_client.private_username);
                                private_mate.private_username = "";
                                private_mate.SendMessage(tcp_client.username + " Disconnected!");
                                private_mate.in_private = false;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        Console.WriteLine(tcp_client.username + " Disconnected");
                        tcp_client.client.Close();
                        break;
                    }
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
            foreach(Client client in clients)
            {
                Console.WriteLine(client.in_group.ToString());
                if (client.username != sender.username && client.in_group)
                {
                    client.SendMessage(message);
                }
            }
        }
    }
}
