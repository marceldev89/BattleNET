using System;
using System.Threading;
using BattleNET;
using System.Diagnostics;

namespace BattleNET_client
{
    class Program
    {
        static void Main(string[] args)
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
            //Thread.Sleep(2000);
            //b.SendCommand(EBattlEyeCommand.Players);
            //Thread.Sleep(2000);
            //b.SendCommand(EBattlEyeCommand.Missions);
            //Thread.Sleep(5000);
            //b.Disconnect();
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
