﻿using Zen.Builder;
using Zen.Game.Model.Npc;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Descriptor
{
    public class IdleNpcDescriptor : NpcDescriptor
    {
        public IdleNpcDescriptor(Npc npc) : base(npc)
        {
            /* Empty. */
        }

        public override void EncodeDescriptor(NpcUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            if (BlockUpdateRequired)
                builder.PutBit(true).PutBits(2, 0);
            else
                builder.PutBit(false);
        }
    }
}