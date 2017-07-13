using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class NpcUpdateMessageEncoder : MessageEncoder<NpcUpdateMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, NpcUpdateMessage message)
        {
            var builder = new GameFrameBuilder(alloc, 32, FrameType.VariableShort);
            var blockBuilder = new GameFrameBuilder(alloc);
            builder.SwitchToBitAccess();

            builder.PutBits(8, message.LocalNpcCount);

            foreach (var descriptor in message.Descriptors)
                descriptor.Encode(message, builder, blockBuilder);

            if (blockBuilder.GetLength() > 0)
            {
                builder.PutBits(15, 32767);
                builder.SwitchToByteAccess();
                builder.PutRawBuilder(blockBuilder);
            }
            else
            {
                builder.SwitchToByteAccess();
            }

            return builder.ToGameFrame();
        }
    }
}