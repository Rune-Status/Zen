using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class ConfigMessageEncoder : MessageEncoder<ConfigMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, ConfigMessage message)
        {
            GameFrameBuilder builder;
            if (message.Value >= -128 && message.Value <= 127)
            {
                builder = new GameFrameBuilder(alloc, 60);
                builder.Put(DataType.Short, DataTransformation.Add, message.Id);
                builder.Put(DataType.Byte, DataTransformation.Negate, message.Value);
            }
            else
            {
                builder = new GameFrameBuilder(alloc, 226);
                builder.Put(DataType.Int, message.Value);
                builder.Put(DataType.Short, DataTransformation.Add, message.Id);
            }
            return builder.ToGameFrame();
        }
    }
}