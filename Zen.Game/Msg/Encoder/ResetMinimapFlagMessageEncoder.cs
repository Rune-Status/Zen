using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class ResetMinimapFlagMessageEncoder : MessageEncoder<ResetMinimapFlagMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, ResetMinimapFlagMessage message)
        {
            return new GameFrameBuilder(alloc, 153).ToGameFrame();
        }
    }
}