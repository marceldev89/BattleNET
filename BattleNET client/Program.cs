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
            IBattleNET b = new BattleNETClient(loginCredentials);
            b.MessageReceivedEvent += DumpMessage;
            b.DisconnectEvent += Disconnected;
            b.Connect();
            Console.ReadLine();
        }

        private static void Disconnected(BattlEyeDisconnectEventArgs args)
        {
            Console.WriteLine("Disconnected!");
        }

        private static void DumpMessage(BattlEyeMessageEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
    }
}