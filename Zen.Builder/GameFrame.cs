using System;
using DotNetty.Buffers;

namespace Zen.Builder
{
    public class GameFrame
    {
        public GameFrame(int opcode, FrameType type, IByteBuffer payload)
        {
            if (type == FrameType.Raw)
                throw new ArgumentException();

            Opcode = opcode;
            Type = type;
            Payload = payload;
        }

        public int Opcode { get; }
        public FrameType Type { get; }
        public IByteBuffer Payload { get; }
    }
}