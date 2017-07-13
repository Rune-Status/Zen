using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Descriptor
{
    public class RunNpcDescriptor : NpcDescriptor
    {
        private readonly Direction _firstDirection;
        private readonly Direction _secondDirection;

        public RunNpcDescriptor(Npc npc) : base(npc)
        {
            _firstDirection = npc.FirstDirection;
            _secondDirection = npc.SecondDirection;
        }

        public override void EncodeDescriptor(NpcUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            builder.PutBit(true);
            builder.PutBits(2, 2);
            builder.PutBit(true);
            builder.PutBits(3, (int) _firstDirection);
            builder.PutBits(3, (int) _secondDirection);
            builder.PutBit(BlockUpdateRequired);
        }
    }
}