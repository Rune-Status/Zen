using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class SkillMessageEncoder : MessageEncoder<SkillMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, SkillMessage message)
        {
            var metadata = message.Metadata;

            return new GameFrameBuilder(alloc, 38)
                .Put(DataType.Byte, DataTransformation.Add, metadata.Level)
                .Put(DataType.Int, DataOrder.Middle, (int) metadata.Experience)
                .Put(DataType.Byte, metadata.Id).ToGameFrame();
        }
    }
}