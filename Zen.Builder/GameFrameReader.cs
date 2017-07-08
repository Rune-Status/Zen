using System;
using System.Collections.Generic;
using System.Reflection;
using DotNetty.Buffers;
using Zen.Util;

namespace Zen.Builder
{
    public class GameFrameReader
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

        public GameFrameReader(GameFrame frame)
        {
            Buffer = frame.Payload;
        }

        public IByteBuffer Buffer { get; }
        public AccessMode Mode { get; private set; } = AccessMode.ByteAccess;
        public int BitIndex { get; private set; }

        public int GetSignedSmart()
        {
            CheckByteAccess();
            var peek = Buffer.GetByte(Buffer.ReaderIndex);
            if (peek < 128)
                return Buffer.ReadByte() - 64;
            return Buffer.ReadShort() - 49152;
        }

        public int GetUnsignedSmart()
        {
            CheckByteAccess();
            var peek = Buffer.GetByte(Buffer.ReaderIndex);
            if (peek < 128)
                return Buffer.ReadByte();
            return Buffer.ReadShort() - 32768;
        }

        public string ReadString()
        {
            CheckByteAccess();
            return Buffer.ReadString();
        }

        public long GetSigned(DataType type, DataOrder order = DataOrder.Big,
            DataTransformation transformation = DataTransformation.None)
        {
            var attr = GetAttr(type);
            if (attr == null)
                throw new Exception("Data type is not valid!");

            var longValue = Get(type, order, transformation);
            if (type == DataType.Long) return longValue;

            var max = (int) (Math.Pow(2, attr.Bytes * 8 - 1) - 1);
            if (longValue > max)
                longValue -= (max + 1) * 2;
            return longValue;
        }

        public long GetUnsigned(DataType type, DataOrder order = DataOrder.Big,
            DataTransformation transformation = DataTransformation.None)
        {
            var longValue = Get(type, order, transformation);
            if (type == DataType.Long)
                throw new NotSupportedException();
            return longValue & unchecked((long) 0xFFFFFFFFFFFFFFFFL);
        }

        private long Get(DataType type, DataOrder order, DataTransformation transformation)
        {
            CheckByteAccess();

            var attr = GetAttr(type);
            if (attr == null)
                throw new Exception("Data type is not valid!");

            long longValue = 0;
            var length = attr.Bytes;

            switch (order)
            {
                case DataOrder.Big:
                    for (var i = length - 1; i >= 0; i--)
                        if (i == 0 && transformation != DataTransformation.None)
                            switch (transformation)
                            {
                                case DataTransformation.Add:
                                    longValue |= (long) (Buffer.ReadByte() - 128) & 0xFF;
                                    break;
                                case DataTransformation.Negate:
                                    longValue |= (long) -Buffer.ReadByte() & 0xFF;
                                    break;
                                case DataTransformation.Subtract:
                                    longValue |= (long) (128 - Buffer.ReadByte()) & 0xFF;
                                    break;
                                default:
                                    throw new ArgumentException("unknown transformation");
                            }
                        else
                            longValue |= (long) (Buffer.ReadByte() & 0xFF) << (i * 8);
                    break;
                case DataOrder.Little:
                    for (var i = 0; i < length; i++)
                        if (i == 0 && transformation != DataTransformation.None)
                            switch (transformation)
                            {
                                case DataTransformation.Add:
                                    longValue |= (long) (Buffer.ReadByte() - 128) & 0xFF;
                                    break;
                                case DataTransformation.Negate:
                                    longValue |= (long) -Buffer.ReadByte() & 0xFF;
                                    break;
                                case DataTransformation.Subtract:
                                    longValue |= (long) (128 - Buffer.ReadByte()) & 0xFF;
                                    break;
                                default:
                                    throw new ArgumentException("unknown transformation");
                            }
                        else
                            longValue |= (long) (Buffer.ReadByte() & 0xFF) << (i * 8);
                    break;
                case DataOrder.Middle:
                    if (transformation != DataTransformation.None)
                        throw new ArgumentException("middle endian cannot be transformed");
                    if (type != DataType.Int)
                        throw new ArgumentException("middle endian can only be used with an integer");
                    longValue |= (long) (Buffer.ReadByte() & 0xFF) << 8;
                    longValue |= (long) Buffer.ReadByte() & 0xFF;
                    longValue |= (long) (Buffer.ReadByte() & 0xFF) << 24;
                    longValue |= (long) (Buffer.ReadByte() & 0xFF) << 16;
                    break;
                case DataOrder.InversedMiddle:
                    if (transformation != DataTransformation.None)
                        throw new ArgumentException("inversed middle endian cannot be transformed");
                    if (type != DataType.Int)
                        throw new ArgumentException("inversed middle endian can only be used with an integer");
                    longValue |= (long) (Buffer.ReadByte() & 0xFF) << 16;
                    longValue |= (long) (Buffer.ReadByte() & 0xFF) << 24;
                    longValue |= (long) Buffer.ReadByte() & 0xFF;
                    longValue |= (long) (Buffer.ReadByte() & 0xFF) << 8;
                    break;
                default:
                    throw new ArgumentException("unknown order");
            }

            return longValue;
        }

        public void GetBytes(byte[] bytes)
        {
            CheckByteAccess();
            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = Buffer.ReadByte();
        }

        public void GetBytes(DataTransformation transformation, byte[] bytes)
        {
            if (transformation == DataTransformation.None)
                GetBytes(bytes);
            else
                for (var i = 0; i < bytes.Length; i++)
                    bytes[i] = (byte) GetSigned(DataType.Byte, transformation: transformation);
        }

        private void GetBytesReverse(IList<byte> bytes)
        {
            CheckByteAccess();
            for (var i = bytes.Count - 1; i >= 0; i--)
                bytes[i] = Buffer.ReadByte();
        }

        public void GetBytesReverse(DataTransformation transformation, byte[] bytes)
        {
            if (transformation == DataTransformation.None)
                GetBytesReverse(bytes);
            else
                for (var i = bytes.Length - 1; i >= 0; i--)
                    bytes[i] = (byte) GetSigned(DataType.Byte, transformation: transformation);
        }

        private static DataTypeAttr GetAttr(DataType t) => (DataTypeAttr) Attribute.GetCustomAttribute(ForValue(t),
            typeof(DataTypeAttr));

        private static MemberInfo ForValue(DataType t) => typeof(DataType).GetField(Enum.GetName(typeof(DataType), t));

        public void SwitchToByteAccess()
        {
            if (Mode == AccessMode.ByteAccess)
                throw new Exception("Already in byte access mode");
            Mode = AccessMode.ByteAccess;
            Buffer.SetReaderIndex((BitIndex + 7) / 8);
        }

        public void SwitchToBitAccess()
        {
            if (Mode == AccessMode.BitAccess)
                throw new Exception("Already in bit access mode");
            Mode = AccessMode.BitAccess;
            BitIndex = Buffer.ReaderIndex * 8;
        }

        public int GetLength()
        {
            CheckByteAccess();
            return Buffer.ReadableBytes;
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

        public int GetBit() => GetBits(1);
        public int GetBit(bool flag) => GetBits(flag ? 1 : 0);

        private int GetBits(int numBits)
        {
            if (numBits <= 0 || numBits > 32)
                throw new Exception("Number of bits must be between 1 and 32 inclusive");

            CheckBitAccess();

            var bytePos = BitIndex >> 3;
            var bitOffset = 8 - (BitIndex & 7);
            var value = 0;
            BitIndex += numBits;

            for (; numBits > bitOffset; bitOffset = 8)
            {
                value += (Buffer.GetByte(bytePos++) & BitMasks[bitOffset]) << (numBits - bitOffset);
                numBits -= bitOffset;
            }

            if (numBits == bitOffset)
                value += Buffer.GetByte(bytePos) & BitMasks[bitOffset];
            else
                value += (Buffer.GetByte(bytePos) >> (bitOffset - numBits)) & BitMasks[numBits];
            return value;
        }
    }
}