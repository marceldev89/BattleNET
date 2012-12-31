/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.1 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2012 by it's authors.                    *
 *  Some rights reserverd. See COPYING.TXT, AUTHORS.TXT.   *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.ComponentModel;

namespace BattleNET
{
    public enum EBattlEyeDisconnectionType
    {
        [Description("Disconnected!")]
        Manual,

        [Description("Disconnected! (Connection timeout)")]
        ConnectionLost,

        [Description("Disconnected! (Socket Exception)")]
        SocketException,

        [Description("Disconnected! (Failed to login)")]
        LoginFailed,

        [Description("Connection failed!")]
        ConnectionFailed
    }
}
