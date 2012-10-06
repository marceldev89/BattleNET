#region

using System;
using System.Net;
using System.Text;
using BattleNET;

#endregion

namespace BattleNET_client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "BattleNET Client";

            BattlEyeLoginCredentials loginCredentials = GetLoginCredentials();

            Console.Title += string.Format(" - {0}:{1}", loginCredentials.Host, loginCredentials.Port);

            IBattleNET b = new BattlEyeClient(loginCredentials);
            b.MessageReceivedEvent += DumpMessage;
            b.DisconnectEvent += Disconnected;
            b.ReconnectOnPacketLoss(true);
            b.Connect();

            while (true)
            {
                string cmd = Console.ReadLine();

                if (cmd == "exit" || cmd == "logout")
                {
                    Environment.Exit(0);
                }

                if (b.IsConnected())
                {
                    b.SendCommandPacket(cmd);
                }
                else
                {
                    Console.WriteLine("Not connected!");
                }
            }
        }

        private static BattlEyeLoginCredentials GetLoginCredentials()
        {
            string ip = "";
            int port = 0;
            string password = "";

            do
            {
                IPAddress value;
                string input;

                Console.Write("Enter IP address: ");
                input = Console.ReadLine();

                if (IPAddress.TryParse(input, out value))
                {
                    ip = value.ToString();
                }
            } while (ip == "");

            do
            {
                int value;
                string input;

                Console.Write("Enter port number: ");
                input = Console.ReadLine();

                if (int.TryParse(input, out value))
                {
                    port = value;
                }
            } while (port == 0);

            do
            {
                Console.Write("Enter RCon password: ");
                string input = Console.ReadLine();

                if (input != "")
                {
                    password = input;
                }
            } while (password == "");

            var loginCredentials = new BattlEyeLoginCredentials
                                       {
                                           Host = ip,
                                           Port = port,
                                           Password = password,
                                       };

            return loginCredentials;
        }

        private static void Disconnected(BattlEyeDisconnectEventArgs args)
        {
            Console.WriteLine(args.Message);
        }

        private static void DumpMessage(BattlEyeMessageEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
    }
}