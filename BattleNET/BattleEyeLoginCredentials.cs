/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.2 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2012 by it's authors.                    *
 *  Some rights reserverd. See COPYING.TXT, AUTHORS.TXT.   *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace BattleNET
{
    public struct BattlEyeLoginCredentials
    {
        public BattlEyeLoginCredentials(string host, int port, string password)
            : this()
        {
            Host = host;
            Port = port;
            Password = password;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
    }
}