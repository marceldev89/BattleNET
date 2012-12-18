# BattleNET #
BattlEye Protocol Library and Client

#### BattleNET Library ####

BattleNET is a C# (.NET) library for the [BattlEye](http://www.battleye.com/) protocol. 

**Usage:**  
[src/BattleNET client/program.cs](https://github.com/ziellos2k/BattleNET/blob/temp/src/BattleNET%20client/Program.cs)  
  
or  
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

#### BattleNET client ####
The BattleNET client basically replicates the [official BE RCon client](http://www.battleye.com/) but uses BattleNET to do all of it's work.  
  
*BattleNET client requires BattleNET.dll*  
  
**Usage:**  
```
BattleNET client.exe -host [ipaddress] -port [portnumber] -password [password] -command [command]
```
```
-host     [ipaddress]   (Optional)
-port     [portnumber]  (Optional)
-password [password]    (Optional)
-command  [command]     (Optional; Requires all other arguments to be specified!)

Note: If no arguments are specified the client will ask for the login details.
```
```
// Standard behavior
BattleNET client.exe
// Login automatically
BattleNET client.exe -host 127.0.0.1 -port 2302 -password 123456789"
// Login automatically, send command and exit again
BattleNET client.exe -host 127.0.0.1 -port 2302 -password 123456789 -command "say -1 Hello World!"
```

#### Changelog ####
20121217.1
* Library didn't reconnect on packetloss

20121217
* Updated predifined commands
* Timeout values decreased for faster connection drop detection
* UTF8 support
* Better handling of dropped packets (client -> server)
* Packet receiving is now asynchronous
* Some other fixes and cleanups
* Client now accepts command line arguments

20120821
* Hackish fix for not handling very long ban files

20120717
* Decreased keep alive packet interval and timeout timer
* Changed Encoding.Default to Encoding.GetEncoding(1252) as Default is 
  variable across systems (also makes it compatible with Linux/mono)

20120706
* Fixed autoreconnect not working

20120704
* License added

20120628  
* Complete BattlEye protocol implementation

#### License ####
[LGPL](https://github.com/ziellos2k/BattleNET/blob/master/LGPL-LICENSE.txt)

#### Authors ####
[Vipeax](https://github.com/Vipeax)  
[ziellos2k](https://github.com/ziellos2k)  