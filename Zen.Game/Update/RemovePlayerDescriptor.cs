using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update
{
    public class RemovePlayerDescriptor : PlayerDescriptor
    {
        public RemovePlayerDescriptor(Player player, int[] tickets) : base(player, tickets)
        {
        }

        public override void EncodeDescriptor(PlayerUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            builder.PutBits(1, 1)
                .PutBits(2, 3);
        }
    }
}