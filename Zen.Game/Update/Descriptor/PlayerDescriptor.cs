using System;
using System.Collections.Generic;
using System.Linq;
using Zen.Builder;
using Zen.Game.Model.Mob;
using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;
using Zen.Game.Update.Block;

namespace Zen.Game.Update.Descriptor
{
    public abstract class PlayerDescriptor
    {
        private readonly Dictionary<Type, PlayerBlock> _blocks = new Dictionary<Type, PlayerBlock>();

        protected PlayerDescriptor(Player player, int[] tickets)
        {
            if (player.Active)
            {
                var id = player.Id;
                var ticket = player.Appearance.TicketId;
                if (tickets[id] != ticket)
                {
                    tickets[id] = ticket;
                    AddBlock(new AppearancePlayerBlock(player));
                }
            }

            if (player.IsChatUpdated())
                AddBlock(new ChatPlayerBlock(player));

            if (player.IsAnimationUpdated())
                AddBlock(new AnimationPlayerBlock(player));

            if (player.IsSpotAnimationUpdated())
                AddBlock(new SpotAnimationPlayerBlock(player));
        }

        protected bool BlockUpdateRequired => _blocks.Count != 0;

        public static PlayerDescriptor Create(Player player, int[] tickets)
        {
            if (player.FirstDirection == Direction.None)
                return new IdlePlayerDescriptor(player, tickets);
            if (player.SecondDirection == Direction.None)
                return new WalkPlayerDescriptor(player, tickets);
            return new RunPlayerDescriptor(player, tickets);
        }

        public void Encode(PlayerUpdateMessage message, GameFrameBuilder builder, GameFrameBuilder blockBuilder)
        {
            EncodeDescriptor(message, builder, blockBuilder);

            if (!BlockUpdateRequired) return;
            var flags = _blocks.Values.Aggregate(0, (current, block) => current | block.Flag);

            if (flags > 0xFF)
            {
                flags |= 0x10;
                blockBuilder.Put(DataType.Short, DataOrder.Little, flags);
            }
            else
            {
                blockBuilder.Put(DataType.Byte, flags);
            }

            EncodeBlock(message, blockBuilder, typeof(ChatPlayerBlock));
            EncodeBlock(message, blockBuilder, typeof(AnimationPlayerBlock));
            EncodeBlock(message, blockBuilder, typeof(AppearancePlayerBlock));
            EncodeBlock(message, blockBuilder, typeof(SpotAnimationPlayerBlock));
        }

        private void EncodeBlock(PlayerUpdateMessage message, GameFrameBuilder builder, Type type)
        {
            if (!_blocks.ContainsKey(type)) return;
            _blocks[type].Encode(message, builder);
        }

        private void AddBlock(PlayerBlock block) => _blocks[block.GetType()] = block;

        public abstract void EncodeDescriptor(PlayerUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder);
    }
}