using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Zen.Net.Update
{
    public class XorEncoder : MessageToByteEncoder<IByteBuffer>
    {
        public int Key { get; set; } = 0;

        protected override void Encode(IChannelHandlerContext context, IByteBuffer message, IByteBuffer output)
        {
            while (message.IsReadable())
                output.WriteByte((message.ReadByte() & 0xFF) ^ Key);
        }
    }
}