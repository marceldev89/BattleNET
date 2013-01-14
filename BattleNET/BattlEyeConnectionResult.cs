/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2013 by it's authors.                    *
 *  Some rights reserved. See COPYING.TXT, AUTHORS.TXT.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.ComponentModel;

namespace BattleNET
{
    public enum BattlEyeConnectionResult
    {
        [Description("Connected!")]
        Success,

        [Description("Host unreachable!")]
        ConnectionFailed,

        [Description("Invalid login details!")]
        InvalidLogin
    }
}
