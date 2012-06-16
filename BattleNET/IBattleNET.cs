using System;

namespace BattleNET
{
    public interface IBattleNET
    {
        EBattlEyeCommandResult SendCommandPacket(string command);
        EBattlEyeCommandResult SendCommandPacket(EBattlEyeCommand command);
        EBattlEyeCommandResult SendCommandPacket(EBattlEyeCommand command, string parameters);
        bool IsConnected();
        EBattlEyeConnectionResult Connect();
        void Disconnect();
        event BattlEyeMessageEventHandler MessageReceivedEvent;
        event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}
