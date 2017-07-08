using System;

namespace Zen.Builder
{
    public enum DataType
    {
        [DataTypeAttr(1)] Byte,
        [DataTypeAttr(2)] Short,
        [DataTypeAttr(3)] TriByte,
        [DataTypeAttr(4)] Int,
        [DataTypeAttr(8)] Long
    }

    public class DataTypeAttr : Attribute
    {
        internal DataTypeAttr(int bytes)
        {
            Bytes = bytes;
        }

        public int Bytes { get; }
    }
}