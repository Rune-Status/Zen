namespace Zen.Net.World
{
    internal class WorldHandshakeMessage
    {
        public WorldHandshakeMessage(int sessionId)
        {
            SessionId = sessionId;
        }

        public int SessionId { get; }
    }
}