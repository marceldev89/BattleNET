using System;

namespace BattleNET
{
    public interface IBattleNET
    {
        EBattlEyeCommandResult SendCommand(string command);
        EBattlEyeCommandResult SendCommand(EBattlEyeCommand command);
        EBattlEyeCommandResult SendCommand(EBattlEyeCommand command, string parameters);
        bool IsConnected();
        EBattlEyeConnectionResult Connect();
        void Disconnect();
        event BattlEyeMessageEventHandler MessageReceivedEvent;
        event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}
