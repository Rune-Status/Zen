using System;
using System.Collections.Generic;
using System.Linq;
using Zen.Builder;
using Zen.Game.Model.Mob;
using Zen.Game.Model.Npc;
using Zen.Game.Msg.Impl;
using Zen.Game.Update.Block;

namespace Zen.Game.Update.Descriptor
{
    public abstract class NpcDescriptor
    {
        private readonly Dictionary<Type, NpcBlock> _blocks = new Dictionary<Type, NpcBlock>();

        protected NpcDescriptor(Npc npc)
        {
            /* TODO Add Npc Blocks. */
        }

        protected bool BlockUpdateRequired => _blocks.Count != 0;

        public static NpcDescriptor Create(Npc npc)
        {
            if (npc.FirstDirection == Direction.None)
                return new IdleNpcDescriptor(npc);
            if (npc.SecondDirection == Direction.None)
                return new WalkNpcDescriptor(npc);
            return new RunNpcDescriptor(npc);
        }

        private void AddBlock(NpcBlock block) => _blocks[block.GetType()] = block;

        public void Encode(NpcUpdateMessage message, GameFrameBuilder builder, GameFrameBuilder blockBuilder)
        {
            EncodeDescriptor(message, builder, blockBuilder);

            if (!BlockUpdateRequired) return;
            var flags = _blocks.Values.Aggregate(0, (current, block) => current | block.Flag);

            if (flags > 0xFF)
            {
                flags |= 0x8;
                blockBuilder.Put(DataType.Short, DataOrder.Little, flags);
            }
            else
            {
                blockBuilder.Put(DataType.Byte, flags);
            }

            /* TODO Encode Npc Blocks. */
        }

        private void EncodeBlock(NpcUpdateMessage message, GameFrameBuilder builder, Type type)
        {
            if (!_blocks.ContainsKey(type)) return;
            _blocks[type].Encode(message, builder);
        }

        public abstract void EncodeDescriptor(NpcUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder);
    }
}