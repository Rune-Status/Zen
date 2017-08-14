using Zen.Game.Model.Player;

namespace Zen.Game.Msg.Impl
{
    public class ContactMessage : IMessage
    {

        public ContactMessage(Player player, int type)
        {
            Player = player;
            Type = type;
        }

        public ContactMessage(Player player, string name, int worldId)
        {
            Player = player;
            Name = name;
            WorldId = worldId;
            Type = UpdateFriendType;
        }

        public const int UpdateStatusType = 0;
        public const int UpdateFriendType = 1;
        public const int IgnoreListType = 2;
        public Player Player { get; }
        public int Type { get; }
        public string Name { get; }
        public int WorldId { get; }
        public bool IsOnline => (WorldId > 0);
    }
}
