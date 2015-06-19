# BattleNET #

BattleNET is a C# (.NET) library and client for the BattlEye protocol.

#### Source code content ####

```
BattleNET           - The library
BattleNET client    - The client
authors.txt         - BattleNET authors
BattleNET.sln       - BattleNET solution
changelog.txt       - Changes made to BattleNET
license.txt         - The LGPL license
README.md           - This file
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
https://github.com/ziellos2k/BattleNET/blob/master/BattleNET%20client/Program.cs
