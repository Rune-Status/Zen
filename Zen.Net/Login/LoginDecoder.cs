using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Zen.Util;

namespace Zen.Net.Login
{
    public class LoginDecoder : ByteToMessageDecoder
    {
        private int _hash, _type, _size;
        private State _state = State.ReadHeader;

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (_state == State.ReadHeader)
            {
                if (input.ReadableBytes < 4)
                    return;

                _state = State.ReadPayload;
                _hash = input.ReadByte() & 0xFF;
                _type = input.ReadByte() & 0xFF;
                _size = input.ReadUnsignedShort();

                if (_type != 16 && _type != 18)
                    throw new IOException("Invalid login type.");
            }

            if (_state != State.ReadPayload) return;
            if (input.ReadableBytes < _size) return;

            var version = input.ReadInt();
            input.ReadByte();
            input.ReadByte();
            input.ReadByte();

            var displayMode = input.ReadByte() & 0xFF;
            input.ReadUnsignedShort();
            input.ReadUnsignedShort();

            input.ReadByte();

            var uid = new byte[24];
            for (var id = 0; id < uid.Length; id++)
                uid[id] = input.ReadByte();

            input.ReadString();

            input.ReadInt();
            input.ReadInt();
            input.ReadShort();

            var crc = new int[28];
            for (var id = 0; id < crc.Length; id++)
                crc[id] = input.ReadInt();

            var encryptedSize = input.ReadByte() & 0xFF;
            /* Normally we would decrypt RSA here, but we have it disabled.. */
            var secureBuffer = input.ReadBytes(encryptedSize);

            var encryptedType = secureBuffer.ReadByte() & 0xFF;
            if (encryptedType != 10)
                throw new IOException("Invalid encrypted block type.");

            var clientSessionKey = secureBuffer.ReadLong();
            var serverSessionKey = secureBuffer.ReadLong();

            var encodedUsername = secureBuffer.ReadLong();
            var username = encodedUsername.DecodeBase37().FormatDisplayName();
            var password = secureBuffer.ReadString();

            if (((encodedUsername >> 16) & 31) != _hash)
                throw new IOException("Username hash mismatch.");

            var reconnecting = _type == 16;
            output.Add(new LoginRequest(reconnecting, username, password, clientSessionKey, serverSessionKey,
                version, crc, displayMode));

            context.Channel.Pipeline.Remove(this);
        }

        private enum State
        {
            ReadHeader,
            ReadPayload
        }
    }
}