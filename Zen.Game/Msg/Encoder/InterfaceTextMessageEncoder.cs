using System;
using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class InterfaceTextMessageEncoder : MessageEncoder<InterfaceTextMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, InterfaceTextMessage message)
        {
            return new GameFrameBuilder(alloc, 171, FrameType.VariableShort)
                .Put(DataType.Int, DataOrder.InversedMiddle, (message.Id << 16) | message.Slot)
                .PutString(message.Text)
                .Put(DataType.Short, DataTransformation.Add, 0)
                .ToGameFrame();
        }
    }
}