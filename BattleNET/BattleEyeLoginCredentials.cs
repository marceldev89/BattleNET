namespace BattleNET
{
    public struct BattleEyeLoginCredentials
    {
        public BattleEyeLoginCredentials(string host, int port, string password)
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