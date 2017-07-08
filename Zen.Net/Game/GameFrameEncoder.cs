using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Zen.Builder;

namespace Zen.Net.Game
{
    public class GameFrameEncoder : MessageToByteEncoder<GameFrame>
    {
        protected override void Encode(IChannelHandlerContext context, GameFrame message, IByteBuffer output)
        {
            var type = message.Type;
            var payload = message.Payload;

            output.WriteByte(message.Opcode);
            switch (type)
            {
                case FrameType.VariableByte:
                    output.WriteByte(payload.ReadableBytes);
                    break;
                case FrameType.VariableShort:
                    output.WriteShort(payload.ReadableBytes);
                    break;
            }

            output.WriteBytes(payload);
        }
    }
}