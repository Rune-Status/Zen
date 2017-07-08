namespace Zen.Net.Login
{
    public class LoginRequest
    {
        public LoginRequest(bool reconnecting, string username, string password, long clientSessionKey,
            long serverSessionKey, int version, int[] crc, int displayMode)
        {
            Reconnecting = reconnecting;
            Username = username;
            Password = password;
            ClientSessionKey = clientSessionKey;
            ServerSessionKey = serverSessionKey;
            Version = version;
            Crc = crc;
            DisplayMode = displayMode;
        }

        public bool Reconnecting { get; }
        public string Username { get; }
        public string Password { get; }
        public long ClientSessionKey { get; }
        public long ServerSessionKey { get; }
        public int Version { get; }
        public int[] Crc { get; }
        public int DisplayMode { get; }
    }
}