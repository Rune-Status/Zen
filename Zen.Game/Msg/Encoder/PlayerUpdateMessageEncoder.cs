using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class PlayerUpdateMessageEncoder : MessageEncoder<PlayerUpdateMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, PlayerUpdateMessage message)
        {
            var builder = new GameFrameBuilder(alloc, 225, FrameType.VariableShort);
            var blockBuilder = new GameFrameBuilder(alloc);
            builder.SwitchToBitAccess();

            message.SelfDescriptor.Encode(message, builder, blockBuilder);
            builder.PutBits(8, message.LocalPlayerCount);

            foreach (var descriptor in message.Descriptors)
                descriptor.Encode(message, builder, blockBuilder);

            if (blockBuilder.GetLength() > 0)
                builder.PutBits(11, 2047)
                    .SwitchToByteAccess()
                    .PutRawBuilder(blockBuilder);
            else
                builder.SwitchToByteAccess();

            return builder.ToGameFrame();
        }
    }
}