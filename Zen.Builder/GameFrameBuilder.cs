using System;
using System.Collections.Generic;
using System.Reflection;
using DotNetty.Buffers;

namespace Zen.Builder
{
    public class GameFrameBuilder
    {
        private static readonly int[] BitMasks =
        {
            0x0, 0x1, 0x3, 0x7,
            0xf, 0x1f, 0x3f, 0x7f,
            0xff, 0x1ff, 0x3ff, 0x7ff,
            0xfff, 0x1fff, 0x3fff, 0x7fff,
            0xffff, 0x1ffff, 0x3ffff, 0x7ffff,
            0xfffff, 0x1fffff, 0x3fffff, 0x7fffff,
            0xffffff, 0x1ffffff, 0x3ffffff, 0x7ffffff,
            0xfffffff, 0x1fffffff, 0x3fffffff, 0x7fffffff,
            -1
        };

        public GameFrameBuilder(IByteBufferAllocator allocator) : this(allocator, -1, FrameType.Raw)
        {
        }

        public GameFrameBuilder(IByteBufferAllocator allocator, int opcode,
            FrameType type = FrameType.Fixed)
        {
            Allocator = allocator;
            Buffer = allocator.Buffer();
            Opcode = opcode;
            Type = type;
        }

        public int BitIndex { get; private set; }
        public AccessMode Mode { get; private set; } = AccessMode.ByteAccess;
        public FrameType Type { get; }
        public int Opcode { get; }
        public IByteBuffer Buffer { get; }
        public IByteBufferAllocator Allocator { get; }

        public GameFrameBuilder Put(DataType type, long value)
        {
            return Put(type, DataOrder.Big, DataTransformation.None, value);
        }

        public GameFrameBuilder Put(DataType type, DataOrder order, long value)
        {
            return Put(type, order, DataTransformation.None, value);
        }

        public GameFrameBuilder Put(DataType type, DataTransformation transformation, long value)
        {
            return Put(type, DataOrder.Big, transformation, value);
        }

        public GameFrameBuilder Put(DataType type, DataOrder order, DataTransformation transformation, long value)
        {
            CheckByteAccess();

            var attr = GetAttr(type);
            if (attr == null)
                throw new Exception("Data type is not valid!");

            var length = attr.Bytes;
            if (order == DataOrder.Big)
            {
                for (var i = length - 1; i >= 0; i--)
                    if (i == 0 && transformation != DataTransformation.None)
                        if (transformation == DataTransformation.Add)
                            Buffer.WriteByte((byte) (value + 128));
                        else if (transformation == DataTransformation.Negate)
                            Buffer.WriteByte((byte) -value);
                        else if (transformation == DataTransformation.Subtract)
                            Buffer.WriteByte((byte) (128 - value));
                        else
                            throw new ArgumentException("Unknown transformation");
                    else
                        Buffer.WriteByte((byte) (value >> (i * 8)));
            }
            else if (order == DataOrder.Little)
            {
                for (var i = 0; i < length; i++)
                    if (i == 0 && transformation != DataTransformation.None)
                        if (transformation == DataTransformation.Add)
                            Buffer.WriteByte((byte) (value + 128));
                        else if (transformation == DataTransformation.Negate)
                            Buffer.WriteByte((byte) -value);
                        else if (transformation == DataTransformation.Subtract)
                            Buffer.WriteByte((byte) (128 - value));
                        else
                            throw new ArgumentException("Unknown transformation");
                    else
                        Buffer.WriteByte((byte) (value >> (i * 8)));
            }
            else if (order == DataOrder.Middle)
            {
                if (transformation != DataTransformation.None)
                    throw new ArgumentException("Middle Endian cannot be transformed");
                if (type != DataType.Int)
                    throw new ArgumentException("Middle Endian can only be used with an integer");
                Buffer.WriteByte((byte) (value >> 8));
                Buffer.WriteByte((byte) value);
                Buffer.WriteByte((byte) (value >> 24));
                Buffer.WriteByte((byte) (value >> 16));
            }
            else if (order == DataOrder.InversedMiddle)
            {
                if (transformation != DataTransformation.None)
                    throw new ArgumentException("Inversed Middle Endian cannot be transformed");
                if (type != DataType.Int)
                    throw new ArgumentException("Inversed Middle Endian can only be used with an integer");
                Buffer.WriteByte((byte) (value >> 16));
                Buffer.WriteByte((byte) (value >> 24));
                Buffer.WriteByte((byte) value);
                Buffer.WriteByte((byte) (value >> 8));
            }
            else
            {
                throw new ArgumentException("Unknown order");
            }
            return this;
        }

        private static DataTypeAttr GetAttr(DataType t) => (DataTypeAttr) Attribute.GetCustomAttribute(ForValue(t),
            typeof(DataTypeAttr));

        private static MemberInfo ForValue(DataType t) => typeof(DataType).GetField(Enum.GetName(typeof(DataType), t));

        public int GetLength()
        {
            CheckByteAccess();
            return Buffer.WriterIndex;
        }

        public GameFrameBuilder PutRawBuilder(GameFrameBuilder builder)
        {
            CheckByteAccess();
            if (builder.Type != FrameType.Raw)
                throw new ArgumentException("Builder must be raw!");
            builder.CheckByteAccess();
            PutBytes(builder.Buffer);
            return this;
        }

        public GameFrameBuilder PutRawBuilderReverse(GameFrameBuilder builder)
        {
            CheckByteAccess();
            if (builder.Type != FrameType.Raw)
                throw new ArgumentException("Builder must be raw!");
            builder.CheckByteAccess();
            return PutBytesReverse(builder.Buffer);
        }

        public GameFrameBuilder PutString(string str)
        {
            CheckByteAccess();
            var chars = str.ToCharArray();
            foreach (var c in chars)
                Buffer.WriteByte((byte) c);
            Buffer.WriteByte(0);
            return this;
        }

        public GameFrameBuilder PutSmart(int value)
        {
            CheckByteAccess();
            if (value < 128)
                Buffer.WriteByte(value);
            else
                Buffer.WriteShort(value);
            return this;
        }

        public GameFrameBuilder PutBytes(IByteBuffer buffer)
        {
            var bytes = new byte[buffer.ReadableBytes];
            buffer.MarkReaderIndex();
            try
            {
                buffer.ReadBytes(bytes);
            }
            finally
            {
                buffer.ResetReaderIndex();
            }
            return PutBytes(bytes);
        }

        public GameFrameBuilder PutBytesReverse(IByteBuffer buffer)
        {
            var bytes = new byte[buffer.ReadableBytes];
            buffer.MarkReaderIndex();
            try
            {
                buffer.ReadBytes(bytes);
            }
            finally
            {
                buffer.ResetReaderIndex();
            }
            return PutBytesReverse(bytes);
        }

        public GameFrameBuilder PutBytes(DataTransformation transformation, byte[] bytes)
        {
            if (transformation == DataTransformation.None)
                return PutBytes(bytes);

            foreach (var b in bytes)
                Put(DataType.Byte, transformation, b);
            return this;
        }

        public GameFrameBuilder PutBytesReverse(DataTransformation transformation, byte[] bytes)
        {
            if (transformation == DataTransformation.None)
                return PutBytesReverse(bytes);

            for (var i = bytes.Length - 1; i >= 0; i--)
                Put(DataType.Byte, transformation, bytes[i]);
            return this;
        }

        public GameFrameBuilder PutBytesReverse(IReadOnlyList<byte> bytes)
        {
            CheckByteAccess();
            for (var i = bytes.Count - 1; i >= 0; i--)
                Buffer.WriteByte(bytes[i]);
            return this;
        }

        public GameFrameBuilder PutBytes(byte[] bytes)
        {
            CheckByteAccess();
            Buffer.WriteBytes(bytes);
            return this;
        }

        public GameFrameBuilder PutBit(bool flag) => PutBit(flag ? 1 : 0);
        private GameFrameBuilder PutBit(int value) => PutBits(1, value);

        public GameFrameBuilder PutBits(int numBits, int value)
        {
            if (numBits <= 0 || numBits > 32)
                throw new Exception("Number of bits must be between 1 and 32 inclusive");

            CheckBitAccess();

            var bytePos = BitIndex >> 3;
            var bitOffset = 8 - (BitIndex & 7);
            BitIndex += numBits;

            var requiredSpace = bytePos - Buffer.WriterIndex + 1;
            requiredSpace += (numBits + 7) / 8;
            Buffer.EnsureWritable(requiredSpace);

            for (; numBits > bitOffset; bitOffset = 8)
            {
                int tmp = Buffer.GetByte(bytePos);
                tmp &= ~BitMasks[bitOffset];
                tmp |= (value >> (numBits - bitOffset)) & BitMasks[bitOffset];
                Buffer.SetByte(bytePos++, tmp);
                numBits -= bitOffset;
            }

            if (numBits == bitOffset)
            {
                int tmp = Buffer.GetByte(bytePos);
                tmp &= ~BitMasks[bitOffset];
                tmp |= value & BitMasks[bitOffset];
                Buffer.SetByte(bytePos, tmp);
            }
            else
            {
                int tmp = Buffer.GetByte(bytePos);
                tmp &= ~(BitMasks[numBits] << (bitOffset - numBits));
                tmp |= (value & BitMasks[numBits]) << (bitOffset - numBits);
                Buffer.SetByte(bytePos, tmp);
            }
            return this;
        }

        public GameFrameBuilder SwitchToByteAccess()
        {
            if (Mode == AccessMode.ByteAccess)
                throw new Exception("Already in byte access mode.");
            Mode = AccessMode.ByteAccess;
            Buffer.SetWriterIndex((BitIndex + 7) / 8);
            return this;
        }

        public GameFrameBuilder SwitchToBitAccess()
        {
            if (Mode == AccessMode.BitAccess)
                throw new Exception("Already in bit access mode.");
            Mode = AccessMode.BitAccess;
            BitIndex = Buffer.WriterIndex * 8;
            return this;
        }

        private void CheckByteAccess()
        {
            if (Mode != AccessMode.ByteAccess)
                throw new Exception("For byte-based calls to work, the mode must be byte access");
        }

        private void CheckBitAccess()
        {
            if (Mode != AccessMode.BitAccess)
                throw new Exception("For bit-based calls to work, the mode must be bit access");
        }

        public GameFrame ToGameFrame()
        {
            if (Type == FrameType.Raw)
                throw new Exception("Raw builders cannot be converted to frames.");

            if (Mode != AccessMode.ByteAccess)
                throw new Exception("Must be in byte access mode to convert to a packet.");

            return new GameFrame(Opcode, Type, Buffer);
        }
    }
}