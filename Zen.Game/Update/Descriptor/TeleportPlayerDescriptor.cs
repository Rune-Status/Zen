using Zen.Builder;
using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Descriptor
{
    public class TeleportPlayerDescriptor : PlayerDescriptor
    {
        private readonly bool _regionChanging;

        public TeleportPlayerDescriptor(Player player, int[] tickets) : base(player, tickets)
        {
            _regionChanging = player.RegionChanging;
        }

        public override void EncodeDescriptor(PlayerUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            var lastKnownRegion = message.LastKnownRegion;
            var position = message.Position;

            var x = position.GetLocalX(lastKnownRegion.CentralRegionX);
            var y = position.GetLocalY(lastKnownRegion.CentralRegionY);
            var height = position.Height;

            builder.PutBits(1, 1)
                .PutBits(2, 3)
                .PutBits(7, y)
                .PutBits(1, _regionChanging ? 0 : 1)
                .PutBits(2, height)
                .PutBits(1, BlockUpdateRequired ? 1 : 0)
                .PutBits(7, x);
        }
    }
}