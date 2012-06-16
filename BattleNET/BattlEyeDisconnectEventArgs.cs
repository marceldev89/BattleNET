using System;

namespace BattleNET
{
    public delegate void BattlEyeDisconnectEventHandler(BattlEyeDisconnectEventArgs args);

    public class BattlEyeDisconnectEventArgs : EventArgs
    {
        public BattlEyeDisconnectEventArgs(BattleEyeLoginCredentials loginDetails, EBattlEyeDisconnectionType disconnectionType)
        {
            LoginDetails = loginDetails;
            DisconnectionType = disconnectionType;
        }

        public BattleEyeLoginCredentials LoginDetails { get; private set; }
        public EBattlEyeDisconnectionType DisconnectionType { get; set; }
    }
}
