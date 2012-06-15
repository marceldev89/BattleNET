using System;

namespace BattleNET
{
    public delegate void BattlEyeDisconnectEventHandler(BattlEyeDisconnectEventArgs args);

    public class BattlEyeDisconnectEventArgs : EventArgs
    {
        public BattlEyeDisconnectEventArgs(BattleEyeLoginCredentials loginDetails, bool unexpected)
        {
            LoginDetails = loginDetails;
            UnexpectedDisconnection = unexpected;
        }

        public BattleEyeLoginCredentials LoginDetails { get; private set; }
        public bool UnexpectedDisconnection { get; set; }
    }
}
