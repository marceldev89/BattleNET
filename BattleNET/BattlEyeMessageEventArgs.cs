/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.1 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2012 by it's authors.                    *
 *  Some rights reserverd. See COPYING.TXT, AUTHORS.TXT.   *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

namespace BattleNET
{
    public delegate void BattlEyeMessageEventHandler(BattlEyeMessageEventArgs args);

    public class BattlEyeMessageEventArgs : EventArgs
    {
        public BattlEyeMessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}