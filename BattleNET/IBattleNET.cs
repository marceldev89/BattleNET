using System;

namespace BattleNET
{
    public interface IBattleNET
    {
        EBattlEyeCommandResult SendCommand(string command);
        bool IsConnected();
        EBattlEyeConnectionResult Connect();
        void Disconnect();
        event BattlEyeMessageEventHandler MessageReceivedEvent;
        event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}
