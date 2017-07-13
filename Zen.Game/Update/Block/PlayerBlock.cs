using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Block
{
    public abstract class PlayerBlock
    {
        protected PlayerBlock(int flag)
        {
            Flag = flag;
        }

        public int Flag { get; }

        public abstract void Encode(PlayerUpdateMessage message, GameFrameBuilder builder);
    }
}