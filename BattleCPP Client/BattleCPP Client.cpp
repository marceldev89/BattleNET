// BattleCPP Client.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "..\BattleCPP\BattleCPP.h"


int _tmain(int argc, _TCHAR* argv[])
{
	BattleCPP::BattlEyeClientWrapper battlEyeClient("127.0.0.1", 2302, "admin");
	battlEyeClient.Connect();
	battlEyeClient.SendCommand("say -1 test123");
	battlEyeClient.Disconnect();
	return 0;
}
