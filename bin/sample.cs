using System;
using BattleNET;

private static void Main(string[] args)
{
	// Setup login data
	BattlEyeLoginCredentials loginCredentials = new BattlEyeLoginCredentials
	{
		Host = "127.0.0.1",
		Port = 2302,
		Password = "password"
	};

	// Create new BattlEyeClient instance
	BattlEyeClient b = new BattlEyeClient(loginCredentials);

	// Event triggered by BattlEye server messages
	b.MessageReceivedEvent += HandleMessage;

	// Event triggered by all disconnect methods
	b.DisconnectEvent += HandleDisconnect;
	
	// Auto reconnect when connection is lost
	b.ReconnectOnPacketLoss(true);

	// Connect
	b.Connect();

	// Send predifined command
	b.SendCommandPacket(EBattlEyeCommand.Lock);

	// Send predifined command with parameters
	b.SendCommandPacket(EBattlEyeCommand.Say, "-1 This is a global message.");

	// Send predifined command with parameters
	b.SendCommandPacket(EBattlEyeCommand.MaxPing, "200");

	// Send custom command
	b.SendCommandPacket("say -1 This is another global message.");

	// Disconnect
	b.Disconnect();
}

private static void HandleMessage(BattlEyeMessageEventArgs args)
{
	// Write sever message to console
	Console.WriteLine(args.Message);
	
}

private static void HandleDisconnect(BattlEyeDisconnectEventArgs args)
{
	// Write disconnect message to console
	Console.WriteLine(args.Message);	
	
	switch (args.DisconnectionType)
	{
		case EBattlEyeDisconnectionType.ConnectionLost:
			// Reconnect
			break;

		case EBattlEyeDisconnectionType.LoginFailed:
			// Wrong BattlEyeLoginCredentials
			break;
			
		case EBattlEyeDisconnectionType.Manual:
			// Disconnect by program
			break;
			
		case EBattlEyeDisconnectionType.SocketException:
			// SocketException
			break;
	}
}