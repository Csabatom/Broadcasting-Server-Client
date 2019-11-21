using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        private static TcpListener tcpListener;
        private static List<TcpClient> tcpClientsList = new List<TcpClient>();
        private static int clientsOnServer = 0;

        static void Main(string[] args)
        {
            Console.Write("Please input, what the port should be: ");
            int port = Convert.ToInt32(Console.ReadLine());

            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            Console.Write(time());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Server started");

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpClientsList.Add(tcpClient);
                Thread thread = new Thread(ClientListener);
                thread.Start(tcpClient);
                StreamWriter sWriter = new StreamWriter(tcpClient.GetStream());
                sWriter.WriteLine(QuestionsIntoArray());
                sWriter.Flush();
                clientsOnServer += 1;
                Thread.Sleep(100);
                Console.Write(time());
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("There are " + clientsOnServer + " clients on the server right now");
                Console.ForegroundColor = ConsoleColor.Blue;
                //tcpClientsList.ForEach(Console.WriteLine);
            }
        }

        public static void ClientListener(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;
            StreamReader reader = new StreamReader(tcpClient.GetStream());

            Console.Write(time());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Client connected");
            while (true)
            {
                try
                {
                    string message = reader.ReadLine();
                    BroadCast(message, tcpClient);
                    Console.Write(time());
                    Console.ForegroundColor = ConsoleColor.White;
                    if (String.IsNullOrEmpty(message))
                    {}
                    else{
                        Console.WriteLine(message);
                    }
                }
                catch (IOException)
                {
                    clientsOnServer -= 1;
                    Console.Write(time());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("A client has been disconnected");
                    Console.Write(time());
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("There are " + clientsOnServer + " clients on the server right now");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                }
            }
        }

        public static void BroadCast(string msg, TcpClient excludeClient)
        {
            foreach (TcpClient client in tcpClientsList)
            {
                if (client != excludeClient)
                {
                    try
                    {
                        StreamWriter sWriter = new StreamWriter(client.GetStream());
                        sWriter.WriteLine(msg);
                        sWriter.Flush();
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("Broadcast not possible");
                    }
                }
            }
        }

        static string QuestionsIntoArray()
        {
            string text = System.IO.File.ReadAllText(@"C:\Users\nyiro\Desktop\Questions.txt");
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\nyiro\Desktop\Questions.txt");
            return text;
        }

        static string time()
        {
            String datetime = Convert.ToString(DateTime.Now);
            string[] splittedTime = datetime.Split(' ');
            datetime = "[" + splittedTime[0] + splittedTime[1] + splittedTime[2] + " " + splittedTime[3] + "] ";
            Console.ForegroundColor = ConsoleColor.Green;
            return datetime;
        }
    }
}