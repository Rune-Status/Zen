using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Descriptor
{
    public class WalkNpcDescriptor : NpcDescriptor
    {
        private readonly Direction _direction;

        public WalkNpcDescriptor(Npc npc) : base(npc)
        {
            _direction = npc.FirstDirection;
        }

        public override void EncodeDescriptor(NpcUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            builder.PutBit(true);
            builder.PutBits(2, 1);
            builder.PutBits(3, (int) _direction);
            builder.PutBit(BlockUpdateRequired);
        }
    }
}