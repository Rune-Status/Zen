namespace Zen.Net.World
{
    public class WorldEntry
    {
        public const int FlagMembers = 0x1;
        public const int FlagQuickChat = 0x2;
        public const int FlagPvp = 0x4;
        public const int FlagLootShare = 0x8;
        public const int FlagHighlight = 0x10;

        public WorldEntry(int id, int flags, int country, string activity, string ip)
        {
            Id = id;
            Flags = flags;
            Country = country;
            Activity = activity;
            Ip = ip;
        }

        public string Ip { get; }
        public string Activity { get; }
        public int Country { get; }
        public int Flags { get; }
        public int Id { get; }
    }
}