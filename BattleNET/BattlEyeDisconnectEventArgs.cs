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

            switch (DisconnectionType)
            {
                case EBattlEyeDisconnectionType.ConnectionLost:
                    Message = "Connection lost!";
                    break;

                case EBattlEyeDisconnectionType.LoginFailed:
                    Message = "Failed to login!";
                    break;
                case EBattlEyeDisconnectionType.Manual:
                    Message = "Disconnected!";
                    break;
                case EBattlEyeDisconnectionType.SocketException:
                    Message = "Socket Exception!";
                    break;
            }
        }

        public BattleEyeLoginCredentials LoginDetails { get; private set; }
        public EBattlEyeDisconnectionType DisconnectionType { get; set; }
        public string Message { get; private set; }
    }
}
