using Zen.Builder;
using Zen.Game.Model.Mob;
using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Descriptor
{
    public class RunPlayerDescriptor : PlayerDescriptor
    {
        private readonly Direction _firstDirection;
        private readonly Direction _secondDirection;

        public RunPlayerDescriptor(Player player, int[] tickets) : base(player, tickets)
        {
            _firstDirection = player.FirstDirection;
            _secondDirection = player.SecondDirection;
        }

        public override void EncodeDescriptor(PlayerUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            builder.PutBits(1, 1)
                .PutBits(2, 2)
                .PutBits(1, 1)
                .PutBits(3, (int) _firstDirection)
                .PutBits(3, (int) _secondDirection)
                .PutBits(1, BlockUpdateRequired ? 1 : 0);
        }
    }
}