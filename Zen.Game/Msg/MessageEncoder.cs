using System;
using DotNetty.Buffers;
using Zen.Builder;

namespace Zen.Game.Msg
{
    public abstract class MessageEncoder<T> where T : IMessage
    {
        public Type MessageType => typeof(T);

        public abstract GameFrame Encode(IByteBufferAllocator alloc, T message);
    }
}