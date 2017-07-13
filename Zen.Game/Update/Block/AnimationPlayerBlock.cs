using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Block
{
    public class AnimationPlayerBlock : PlayerBlock
    {
        private readonly Animation _animation;

        public AnimationPlayerBlock(Player player) : base(0x8)
        {
            _animation = player.Animation;
        }

        public override void Encode(PlayerUpdateMessage message, GameFrameBuilder builder)
        {
            builder.Put(DataType.Short, _animation.Id);
            builder.Put(DataType.Byte, _animation.Delay);
        }
    }
}