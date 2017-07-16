using Zen.Game.Model;
using Zen.Game.Model.Player;

namespace Zen.Game.Msg.Impl
{
    public class RegionChangeMessage : IMessage
    {
        public RegionChangeMessage(Player player)
        {
            Position = player.Position;
            player.SetLastKnownRegion(Position);
        }

        public Position Position { get; }
    }
}