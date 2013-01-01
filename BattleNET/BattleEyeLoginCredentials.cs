/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.2 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2013 by it's authors.                    *
 *  Some rights reserved. See COPYING.TXT, AUTHORS.TXT.    *
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