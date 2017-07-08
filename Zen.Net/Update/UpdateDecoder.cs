using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Zen.Net.Update
{
    public class UpdateDecoder : ByteToMessageDecoder
    {
        private State _state = State.ReadVersion;

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (input.ReadableBytes < 4)
                return;

            if (_state == State.ReadVersion)
            {
                _state = State.ReadRequest;
                output.Add(new UpdateVersionMessage(input.ReadInt()));
            }
            else
            {
                var opcode = input.ReadByte() & 0xFF;

                switch (opcode)
                {
                    case 0:
                    case 1:
                        var type = input.ReadByte() & 0xFF;
                        var file = input.ReadUnsignedShort();

                        output.Add(new FileRequest(opcode == 1, type, file));
                        break;
                    case 4:
                        var key = input.ReadByte() & 0xFF;
                        input.SetReaderIndex(input.ReaderIndex + 4);
                        output.Add(new UpdateEncryptionMessage(key));
                        break;
                    default:
                        input.SetReaderIndex(input.ReaderIndex + 3);
                        break;
                }
            }
        }

        private enum State
        {
            ReadVersion,
            ReadRequest
        }
    }
}