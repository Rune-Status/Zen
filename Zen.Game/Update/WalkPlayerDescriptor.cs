using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update
{
    public class WalkPlayerDescriptor : PlayerDescriptor
    {
        private readonly Direction _direction;

        public WalkPlayerDescriptor(Player player, int[] tickets) : base(player, tickets)
        {
            _direction = player.FirstDirection;
        }

        public override void EncodeDescriptor(PlayerUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            builder.PutBits(1, 1)
                .PutBits(2, 1)
                .PutBits(3, (int) _direction)
                .PutBits(1, BlockUpdateRequired ? 1 : 0);
        }
    }
}