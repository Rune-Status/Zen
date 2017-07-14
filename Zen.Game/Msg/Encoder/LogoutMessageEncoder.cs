using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class LogoutMessageEncoder : MessageEncoder<LogoutMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, LogoutMessage message)
        {
            return new GameFrameBuilder(alloc, 86).ToGameFrame();
        }
    }
}