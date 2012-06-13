using System;

namespace BattleNET
{
    public interface IBattleNET
    {
        EBattlEyeCommandResult SendCommand(string command);
        bool IsConnected();
        EBattlEyeConnectionResult Connect();
        event BattlEyeMessageEventHandler MessageReceivedEvent;
        event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}
