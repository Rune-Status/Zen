namespace Zen.Net.World
{
    public class WorldListMessage
    {
        public WorldListMessage(int sessionId, Country[] countries, WorldEntry[] worldsEntry, int[] players)
        {
            SessionId = sessionId;
            Countries = countries;
            WorldsEntry = worldsEntry;
            Players = players;
        }

        public int[] Players { get; }
        public WorldEntry[] WorldsEntry { get; }
        public Country[] Countries { get; }
        public int SessionId { get; }
    }
}