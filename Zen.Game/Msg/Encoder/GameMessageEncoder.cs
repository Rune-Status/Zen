using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class GameMessageEncoder : MessageEncoder<GameMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, GameMessage message)
        {
            return new GameFrameBuilder(alloc, 70, FrameType.VariableByte).PutString(message.Message).ToGameFrame();
        }
    }
}