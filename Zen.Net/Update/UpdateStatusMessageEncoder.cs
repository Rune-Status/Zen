using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Zen.Net.Update
{
    public class UpdateStatusMessageEncoder : MessageToByteEncoder<UpdateStatusMessage>
    {
        protected override void Encode(IChannelHandlerContext context, UpdateStatusMessage message, IByteBuffer output)
        {
            output.WriteByte(message.Status);
        }
    }
}