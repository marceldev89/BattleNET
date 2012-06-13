using System;

namespace BattleNET
{
    public delegate void BattlEyeDisconnectEventHandler(BattlEyeDisconnectEventArgs args);

    public class BattlEyeDisconnectEventArgs : EventArgs
    {
        public BattlEyeDisconnectEventArgs(BattleEyeLoginCredentials loginDetails)
        {
            LoginDetails = loginDetails;
        }

        public BattleEyeLoginCredentials LoginDetails { get; private set; }
    }
}
