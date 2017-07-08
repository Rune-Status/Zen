using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Zen.Net.Update
{
    public class FileResponseEncoder : MessageToByteEncoder<FileResponse>
    {
        protected override void Encode(IChannelHandlerContext context, FileResponse message, IByteBuffer output)
        {
            var container = message.Container;
            var type = message.Type;
            var file = message.File;

            output.WriteByte(type);
            output.WriteShort(file);

            var compression = container.ReadByte() & 0xFF;
            if (!message.Priority)
                compression |= 0x80;

            output.WriteByte(compression);

            var bytes = container.ReadableBytes;
            if (bytes > 508)
                bytes = 508;

            output.WriteBytes(container.ReadBytes(bytes));

            while ((bytes = container.ReadableBytes) != 0)
            {
                if (bytes > 511)
                    bytes = 511;

                output.WriteByte(0xFF);
                output.WriteBytes(container.ReadBytes(bytes));
            }
        }
    }
}