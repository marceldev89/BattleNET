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