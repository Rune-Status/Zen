using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class InterfaceRootMessageEncoder : MessageEncoder<InterfaceRootMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, InterfaceRootMessage message)
        {
            return new GameFrameBuilder(alloc, 145)
                .Put(DataType.Short, DataOrder.Little, DataTransformation.Add, message.Interface.Id)
                .Put(DataType.Byte, DataTransformation.Add, 0)
                .Put(DataType.Short, DataOrder.Little, DataTransformation.Add, 0)
                .ToGameFrame();
        }
    }
}