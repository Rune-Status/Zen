using Zen.Game.Model;

namespace Zen.Game.Msg.Impl
{
    public class RegionChangeMessage : Message
    {
        public RegionChangeMessage(Player player)
        {
            Position = player.Position;
            player.SetLastKnownRegion(Position);
        }

        public Position Position { get; }
    }
}