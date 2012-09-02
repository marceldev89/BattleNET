#region

using System.ComponentModel;

#endregion

namespace BattleNET
{
    public enum EBattlEyeCommand
    {
        [Description("say ")] Say,
        [Description("missions")] Missions,
        [Description("players")] Players,
        [Description("kick ")] Kick,
        [Description("RConPassword ")] RConPassword,
        [Description("MaxPing ")] MaxPing,
        [Description("logout")] Logout,
        [Description("Exit")] Exit,
        [Description("#restart")] Restart,
        [Description("#reassign")] Reassign,
        [Description("#shutdown")] Shutdown,
        [Description("#init")] Init,
        [Description("#exec ban ")] ExecBan,
        [Description("#lock ")] Lock,
        [Description("#unlock")] Unlock,
        [Description("loadBans")] loadBans,
        [Description("loadScripts")] loadScripts,
        [Description("loadEvents")] loadEvents
    }
}