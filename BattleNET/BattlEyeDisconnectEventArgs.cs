/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2013 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

namespace BattleNET
{
    public delegate void BattlEyeConnectEventHandler(BattlEyeConnectEventArgs args);
    public delegate void BattlEyeDisconnectEventHandler(BattlEyeDisconnectEventArgs args);

    public class BattlEyeConnectEventArgs : EventArgs
    {
        public BattlEyeConnectEventArgs(BattlEyeLoginCredentials loginDetails, BattlEyeConnectionResult connectionResult)
        {
            LoginDetails = loginDetails;
            ConnectionResult = connectionResult;
            Message = Helpers.StringValueOf(connectionResult);
        }

        public BattlEyeLoginCredentials LoginDetails { get; private set; }
        public BattlEyeConnectionResult ConnectionResult { get; private set; }
        public string Message { get; private set; }
    }
    
    public class BattlEyeDisconnectEventArgs : EventArgs
    {
        public BattlEyeDisconnectEventArgs(BattlEyeLoginCredentials loginDetails, BattlEyeDisconnectionType? disconnectionType)
        {
            LoginDetails = loginDetails;
            DisconnectionType = disconnectionType;
            Message = Helpers.StringValueOf(disconnectionType);
        }

        public BattlEyeLoginCredentials LoginDetails { get; private set; }
        public BattlEyeDisconnectionType? DisconnectionType { get; private set; }
        public string Message { get; private set; }
    }
}
