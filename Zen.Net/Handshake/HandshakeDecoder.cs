using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Zen.Net.Login;
using Zen.Net.Update;
using Zen.Net.World;

namespace Zen.Net.Handshake
{
    public class HandshakeDecoder : ChannelHandlerAdapter
    {
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buf = message as IByteBuffer;
            if (buf == null || !buf.IsReadable()) return;

            try
            {
                var serviceId = buf.ReadByte() & 0xFF;

                var pipeline = context.Channel.Pipeline;
                pipeline.Remove(this);

                switch (serviceId)
                {
                    case HandshakeMessage.ServiceUpdate:
                        pipeline.AddFirst(
                            new FileResponseEncoder(),
                            new UpdateStatusMessageEncoder(),
                            new XorEncoder(),
                            new UpdateDecoder());
                        break;
                    case HandshakeMessage.ServiceLogin:
                        pipeline.AddFirst(
                            new LoginEncoder(),
                            new LoginDecoder());
                        break;
                    case HandshakeMessage.ServiceWorldList:
                        pipeline.AddFirst(
                            new WorldListEncoder(),
                            new WorldListDecoder());
                        break;
                }

                pipeline.FireChannelRead(new HandshakeMessage(serviceId));
                if (buf.IsReadable())
                    pipeline.FireChannelRead(buf.ReadBytes(buf.ReadableBytes));
            }
            finally
            {
                buf.Release();
            }
        }
    }
}