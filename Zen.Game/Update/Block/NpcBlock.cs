using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Block
{
    public abstract class NpcBlock
    {
        protected NpcBlock(int flag)
        {
            Flag = flag;
        }

        public int Flag { get; }

        public abstract void Encode(NpcUpdateMessage message, GameFrameBuilder builder);
    }
}