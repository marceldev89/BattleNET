using System;

namespace BattleNET
{
    public delegate void BattlEyeDisconnectEventHandler(BattlEyeDisconnectEventArgs args);

    public class BattlEyeDisconnectEventArgs : EventArgs
    {
        public BattlEyeDisconnectEventArgs(BattleEyeLoginCredentials loginDetails, bool manual)
        {
            LoginDetails = loginDetails;
            DisconnectedManually = manual;
        }

        public BattleEyeLoginCredentials LoginDetails { get; private set; }
        public bool DisconnectedManually { get; set; }
    }
}
