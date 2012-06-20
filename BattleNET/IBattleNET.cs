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
        bool ReconnectOnPacketLoss(bool newSetting);
        event BattlEyeMessageEventHandler MessageReceivedEvent;
        event BattlEyeDisconnectEventHandler DisconnectEvent;
    }
}
