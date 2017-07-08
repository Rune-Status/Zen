using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update
{
    public class IdlePlayerDescriptor : PlayerDescriptor
    {
        public IdlePlayerDescriptor(Player player, int[] tickets) : base(player, tickets)
        {
        }

        public override void EncodeDescriptor(PlayerUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            if (BlockUpdateRequired)
                builder.PutBits(1, 1)
                    .PutBits(2, 0);
            else
                builder.PutBits(1, 0);
        }
    }
}