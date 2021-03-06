﻿using Zen.Builder;
using Zen.Game.Model.Mob;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Block
{
    public class SpotAnimationPlayerBlock : PlayerBlock
    {
        private readonly SpotAnimation _spotAnimation;

        public SpotAnimationPlayerBlock(Mob player) : base(0x100)
        {
            _spotAnimation = player.SpotAnimation;
        }

        public override void Encode(PlayerUpdateMessage message, GameFrameBuilder builder)
        {
            builder.Put(DataType.Short, DataOrder.Little, _spotAnimation.Id);
            builder.Put(DataType.Int, DataOrder.InversedMiddle, (_spotAnimation.Height << 16) | _spotAnimation.Delay);
        }
    }
}