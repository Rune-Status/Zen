using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class InterfaceOpenMessageEncoder : MessageEncoder<InterfaceOpenMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, InterfaceOpenMessage message)
        {
            return new GameFrameBuilder(alloc, 155)
                .Put(DataType.Byte, message.Transparent ? 1 : 0)
                .Put(DataType.Int, DataOrder.InversedMiddle, message.BitpackedId)
                .Put(DataType.Short, DataTransformation.Add, 0)
                .Put(DataType.Short, message.Id)
                .ToGameFrame();
        }
    }
}