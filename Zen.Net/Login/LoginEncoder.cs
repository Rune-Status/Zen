using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Zen.Shared;

namespace Zen.Net.Login
{
    public class LoginEncoder : MessageToByteEncoder<LoginResponse>
    {
        protected override void Encode(IChannelHandlerContext context, LoginResponse message, IByteBuffer output)
        {
            output.WriteByte(message.Status);
            output.WriteBytes(message.Payload);

            if (message.Status != LoginConstants.StatusExchangeKeys)
                context.Channel.Pipeline.Remove(this);
        }
    }
}