/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.2 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2012 by it's authors.                    *
 *  Some rights reserverd. See COPYING.TXT, AUTHORS.TXT.   *
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
            BattlEyeLoginCredentials loginCredentials;
            string command = "";

            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "BattleNET Client";

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
                    Console.Read();
                    Environment.Exit(0);
                }
            }
            else
            {
                loginCredentials = GetLoginCredentials();
            }

            Console.Title += string.Format(" - {0}:{1}", loginCredentials.Host, loginCredentials.Port);

            BattlEyeClient b = new BattlEyeClient(loginCredentials);
            b.MessageEvent += BattlEyeMessage;
            b.ConnectedEvent += Connected;
            b.DisconnectedEvent += Disconnected;
            b.ReconnectOnPacketLoss = true;
            b.Connect();

            if (command != "")
            {
                b.SendCommandPacket(command);
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
                        b.SendCommandPacket(cmd);
                    }
                    else
                    {
                        Console.WriteLine("Not connected!");
                    }
                }
            }

            b.Disconnect();
        }

        private static BattlEyeLoginCredentials GetLoginCredentials(string[] args)
        {
            BattlEyeLoginCredentials loginCredentials = new BattlEyeLoginCredentials();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-host":
                        {
                            IPAddress value;
                            if (IPAddress.TryParse(args[i + 1], out value))
                            {
                                loginCredentials.Host = value.ToString();
                            }
                            else
                            {
                                Console.WriteLine("No valid host given!", args[i + 1]);
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
                                Console.WriteLine("No valid port given!", args[i + 1]);
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

        private static void Connected(BattlEyeConnectEventArgs args)
        {
            // Connected event
        }

        private static void Disconnected(BattlEyeDisconnectEventArgs args)
        {
            // Disconnected event
        }

        private static void BattlEyeMessage(BattlEyeMessageEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
    }
}
