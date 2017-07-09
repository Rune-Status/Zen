using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zen.Util
{
    public static class BinaryUtil
    {
        public static void WriteByte(this BinaryWriter writer, int value) => writer.BaseStream.WriteByte((byte)value);

        public static void WriteShort(this BinaryWriter writer, int value)
        {
            WriteByte(writer, value >> 8);
            WriteByte(writer, value);
        }

        public static int ReadByte(this BinaryReader reader) => reader.BaseStream.ReadByte();

        public static int ReadShort(this BinaryReader reader) => (ReadByte(reader) << 8) + ReadByte(reader);
    }
}
