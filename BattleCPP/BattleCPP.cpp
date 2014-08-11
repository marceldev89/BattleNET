// This is the main DLL file.

#include "stdafx.h"

#include "BattleCPP.h"

#using "BattleNET.dll"
#include <msclr\auto_gcroot.h>

namespace BattleCPP {
	class BattlEyeClientWrapperPrivate
	{
	public:
		msclr::auto_gcroot<BattleNET::BattlEyeClient^> battleyeClient;
	};

	BattlEyeClientWrapper::BattlEyeClientWrapper(const char* host, int port, const char* password)
	{
		_private = new BattlEyeClientWrapperPrivate();
		_private->battleyeClient = gcnew BattleNET::BattlEyeClient(gcnew System::String(host), port, gcnew System::String(password));
	}

	BattlEyeClientWrapper::~BattlEyeClientWrapper()
	{
		delete _private;
	}

	void BattlEyeClientWrapper::Connect()
	{
		_private->battleyeClient->Connect();
	}

	void BattlEyeClientWrapper::Disconnect()
	{
		_private->battleyeClient->Disconnect();
	}

	void BattlEyeClientWrapper::SendCommand(const char* command)
	{
		_private->battleyeClient->SendCommand(gcnew System::String(command), true);
	}
}

