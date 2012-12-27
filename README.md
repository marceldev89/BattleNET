# BattleNET #

BattleNET is a C# (.NET) library and client for the BattlEye protocol.

#### Source code content ####

```
BattleNET           - The library
BattleNET client    - The client
AUTHORS.txt         - BattleNET authors
BattleNET.sln       - BattleNET solution
CHANGELOG.txt       - Changes made to BattleNET
COPYING.txt         - The LGPL license
README.md          - This file
```

#### BattleNET client ####

The BattleNET client basically replicates the official BE RCon client but uses the BattleNET library to do all of it's work.

Usage:

```
BattleNET client.exe -host [ipaddress] -port [portnumber] -password [password] -command [command]
```
Command line options:
```
-host           [ipaddress]     RCon ip address
-port           [portnumber]    RCon port number
-password       [password]      RCon password
-command        [command]       Sends command to RCon server and exits again
Note: If no arguments are specified the client will ask for the login details.
```

Examples:

```
BattleNET client.exe -host 127.0.0.1 -port 2302 -password 123456789
BattleNET client.exe -host 127.0.0.1 -port 2302 -password 123456789 -command "say -1 Hello World!"
```

#### BattleNET library ####

Implementation sample:

```csharp
using System;
using BattleNET;

private static void Main(string[] args)
{
    BattlEyeLoginCredentials loginCredentials = new BattlEyeLoginCredentials
	{
		Host = "127.0.0.1",
		Port = 2302,
		Password = "password"
	};

	BattlEyeClient b = new BattlEyeClient(loginCredentials);
	b.MessageReceivedEvent += HandleMessage;
	b.DisconnectEvent += HandleDisconnect;
	b.ReconnectOnPacketLoss(true);
	b.Connect();
	
	b.SendCommandPacket("say -1 This is global message.");
	b.SendCommandPacket(EBattlEyeCommand.Say, "-1 This is a another global message.");
	while (b.CommandQueue > 0) { /* wait until server received packet(s) */ }
	b.Disconnect();
}

private static void HandleMessage(BattlEyeMessageEventArgs args)
{
	Console.WriteLine(args.Message);
}

private static void HandleDisconnect(BattlEyeDisconnectEventArgs args)
{
	Console.WriteLine(args.Message);	
}
```
