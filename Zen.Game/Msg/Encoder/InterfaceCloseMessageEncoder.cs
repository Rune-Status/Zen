using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class InterfaceCloseMessageEncoder : MessageEncoder<InterfaceCloseMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, InterfaceCloseMessage message)
        {
            return new GameFrameBuilder(alloc, 0)
                .Put(DataType.Short, 0)
                .Put(DataType.Int, (message.Interface.Parent.Id << 16) | message.Interface.Slot)
                .ToGameFrame();
        }
    }
}