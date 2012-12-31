/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.1 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2012 by it's authors.                    *
 *  Some rights reserverd. See COPYING.TXT, AUTHORS.TXT.   *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

namespace BattleNET
{
    public delegate void BattlEyeDisconnectEventHandler(BattlEyeDisconnectEventArgs args);

    public class BattlEyeDisconnectEventArgs : EventArgs
    {
        public BattlEyeDisconnectEventArgs(BattlEyeLoginCredentials loginDetails,
                                           EBattlEyeDisconnectionType disconnectionType)
        {
            LoginDetails = loginDetails;
            DisconnectionType = disconnectionType;
        }

        public BattlEyeLoginCredentials LoginDetails { get; private set; }
        public EBattlEyeDisconnectionType DisconnectionType { get; set; }
    }
}