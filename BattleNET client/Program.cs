/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2013 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Net;
using System.Text;
using BattleNET;

namespace BattleNET_client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(
                "BattleNET v1.3 - BattlEye Library and Client\n\n" +
                "Copyright (C) 2013 by it's authors.\n" +
                "Some rights reserved. See license.txt, authors.txt.\n"
            );

            BattlEyeLoginCredentials loginCredentials;
            string command = "";

            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length > 0)
            {
                loginCredentials = GetLoginCredentials(args);

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-command")
                    {
                        try
                        {
                            command = args[i + 1];
                        }
                        catch
                        {
                            Console.WriteLine("No command given!");
                            loginCredentials.Host = null;
                        }
                    }
                }

                if (loginCredentials.Host == null || loginCredentials.Port == 0 || loginCredentials.Password == "")
                {
                    Console.WriteLine("BattleNET client usage:");
                    Console.WriteLine("BattleNET client.exe -host 127.0.0.1 -port 2302 -password admin [-command shutdown]");
                    Console.Read();
                    Environment.Exit(0);
                }
            }
            else
            {
                loginCredentials = GetLoginCredentials();
            }

            Console.Title = string.Format("BattleNET client v1.3 - {0}:{1}", loginCredentials.Host, loginCredentials.Port);

            BattlEyeClient b = new BattlEyeClient(loginCredentials);
            b.BattlEyeMessageReceived += BattlEyeMessageReceived;
            b.BattlEyeConnected += BattlEyeConnected;
            b.BattlEyeDisconnected += BattlEyeDisconnected;
            b.ReconnectOnPacketLoss = true;
            b.Connect();

            if (command != "")
            {
                b.SendCommand(command);
                while (b.CommandQueue > 0) { /* wait until server received packet */ };
            }
            else
            {
                while (true)
                {
                    string cmd = Console.ReadLine();

                    if (cmd == "exit" || cmd == "logout")
                    {
                        break;
                    }

                    if (b.Connected)
                    {
                        b.SendCommand(cmd);
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
            }

            b.Disconnect();
        }

        private static void BattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            //if (args.ConnectionResult == BattlEyeConnectionResult.Success) { /* Connected successfully */ }
            //if (args.ConnectionResult == BattlEyeConnectionResult.InvalidLogin) { /* Connection failed, invalid login details */ }
            //if (args.ConnectionResult == BattlEyeConnectionResult.ConnectionFailed) { /* Connection failed, host unreachable */ }

            Console.WriteLine(args.Message);
        }

        private static void BattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            //if (args.DisconnectionType == BattlEyeDisconnectionType.ConnectionLost) { /* Connection lost (timeout), if ReconnectOnPacketLoss is set to true it will reconnect */ }
            //if (args.DisconnectionType == BattlEyeDisconnectionType.SocketException) { /* Something went terribly wrong... */ }
            //if (args.DisconnectionType == BattlEyeDisconnectionType.Manual) { /* Disconnected by implementing application, that would be you */ }

            Console.WriteLine(args.Message);
        }

        private static void BattlEyeMessageReceived(BattlEyeMessageEventArgs args)
        {
            //if (args.Id == playerListId)
            //{
            //    playerList = args.Message;
            //}

            Console.WriteLine(args.Message);
        }

        private static BattlEyeLoginCredentials GetLoginCredentials(string[] args)
        {
            BattlEyeLoginCredentials loginCredentials = new BattlEyeLoginCredentials();
            loginCredentials.Host = null;
            loginCredentials.Port = 0;
            loginCredentials.Password = "";

            for (int i = 0; i < args.Length; i = i + 2)
            {
                switch (args[i])
                {
                    case "-host":
                        {
                            try
                            {
                                IPAddress ip = Dns.GetHostAddresses(args[i + 1])[0];
                                loginCredentials.Host = ip;
                            }
                            catch
                            {
                                Console.WriteLine("No valid host given!");
                            }
                            break;
                        }

                    case "-port":
                        {
                            int value;
                            if (int.TryParse(args[i + 1], out value))
                            {
                                loginCredentials.Port = value;
                            }
                            else
                            {
                                Console.WriteLine("No valid port given!");
                            }
                            break;
                        }

                    case "-password":
                        {
                            if (args[i + 1] != "")
                            {
                                loginCredentials.Password = args[i + 1];
                            }
                            else
                            {
                                Console.WriteLine("No password given!");
                            }
                            break;
                        }
                }
            }

            return loginCredentials;
        }

        private static BattlEyeLoginCredentials GetLoginCredentials()
        {
            IPAddress host = null;
            int port = 0;
            string password = "";

            do
            {
                string input;
                Console.Write("Enter IP address or hostname: ");
                input = Console.ReadLine();

                try
                {
                    IPAddress ip = Dns.GetHostAddresses(input)[0];
                    host = ip;
                }
                catch { /* try again */ }
            } while (host == null);

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
                                           Host = host,
                                           Port = port,
                                           Password = password,
                                       };

            return loginCredentials;
        }
    }
}
