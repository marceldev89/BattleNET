// BattleCPP.h

#pragma once

//using namespace System;

namespace BattleCPP {
	class BattlEyeClientWrapperPrivate;

	class __declspec(dllexport) BattlEyeClientWrapper
	{
	private:
		BattlEyeClientWrapperPrivate* _private;
	public:
		BattlEyeClientWrapper(const char*, int, const char*);
		~BattlEyeClientWrapper();
		void Connect();
		void Disconnect();
		void SendCommand(const char*);
	};
}
