using System;
using BattleNET;

namespace BattleNET_client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BattleEyeLoginCredentials loginCredentials = new BattleEyeLoginCredentials
                                                             {
                                                                 Host = "109.236.85.132",
                                                                 Port = 2402,
                                                                 Password = "arbeiten",
                                                             };
            IBattleNET b = new IBattlEyeClient(loginCredentials);
            b.MessageReceivedEvent += DumpMessage;
            b.DisconnectEvent += Disconnected;
            b.Connect();

            while (true)
            {
                string cmd = Console.ReadLine();
                b.SendCommandPacket(cmd);
            }
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