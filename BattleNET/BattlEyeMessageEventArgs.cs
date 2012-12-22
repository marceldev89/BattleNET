#region

using System;

#endregion

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