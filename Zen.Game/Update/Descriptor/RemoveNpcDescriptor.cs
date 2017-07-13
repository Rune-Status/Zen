using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Descriptor
{
    public class RemoveNpcDescriptor : NpcDescriptor
    {
        public RemoveNpcDescriptor(Npc npc) : base(npc)
        {
            /* Empty. */
        }

        public override void EncodeDescriptor(NpcUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            builder.PutBit(true).PutBits(2, 3);
        }
    }
}