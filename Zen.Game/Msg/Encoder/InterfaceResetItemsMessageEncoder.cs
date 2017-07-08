using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class InterfaceResetItemsMessageEncoder : MessageEncoder<InterfaceResetItemsMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, InterfaceResetItemsMessage message)
        {
            return new GameFrameBuilder(alloc, 144)
                .Put(DataType.Int, DataOrder.InversedMiddle, (message.Id << 16) | message.Slot)
                .ToGameFrame();
        }
    }
}