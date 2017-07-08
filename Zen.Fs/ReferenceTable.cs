using System.Collections.Generic;
using System.Linq;
using DotNetty.Buffers;
using Zen.Util;

namespace Zen.Fs
{
    public class ReferenceTable
    {
        public const int FlagIdentifiers = 0x01;
        public const int FlagWhirlpool = 0x02;
        public Dictionary<int, int> NamedEntries = new Dictionary<int, int>();

        public int Format { get; private set; }
        public int Version { get; private set; }
        public int Flags { get; private set; }
        public SortedDictionary<int, Entry> Entries { get; } = new SortedDictionary<int, Entry>();

        public int Capacity => Entries.Count == 0 ? 0 : Entries.Keys.Last() + 1;
        public int Size => Entries.Count;
        public Entry GetEntry(int key) => Entries.ContainsKey(key) ? Entries[key] : null;

        public static ReferenceTable Decode(IByteBuffer buffer)
        {
            var table = new ReferenceTable {Format = buffer.ReadByte() & 0xFF};
            if (table.Format >= 6)
                table.Version = buffer.ReadInt();

            table.Flags = buffer.ReadByte() & 0xFF;

            var ids = new int[buffer.ReadShort() & 0xFFFF];
            int accumulator = 0, size = -1;
            for (var id = 0; id < ids.Length; id++)
            {
                var delta = buffer.ReadShort() & 0xFFFF;
                ids[id] = accumulator += delta;
                if (ids[id] > size)
                    size = ids[id];
            }
            size++;

            foreach (var id in ids)
                table.Entries[id] = new Entry();

            if ((table.Flags & FlagIdentifiers) != 0)
                foreach (var id in ids)
                {
                    var identifier = table.Entries[id].Identifier = buffer.ReadInt();
                    table.NamedEntries[identifier] = id;
                }

            foreach (var id in ids)
                table.Entries[id].Crc = buffer.ReadInt();

            if ((table.Flags & FlagWhirlpool) != 0)
                foreach (var id in ids)
                    buffer.ReadBytes(table.Entries[id].Whirlpool);

            foreach (var id in ids)
                table.Entries[id].Version = buffer.ReadInt();

            var members = new int[size][];
            foreach (var id in ids)
                members[id] = new int[buffer.ReadShort() & 0xFFFF];

            foreach (var id in ids)
            {
                accumulator = 0;
                size = -1;

                for (var i = 0; i < members[id].Length; i++)
                {
                    var delta = buffer.ReadShort() & 0xFFFF;
                    members[id][i] = accumulator += delta;
                    if (members[id][i] > size)
                        size = members[id][i];
                }

                foreach (var child in members[id])
                    table.Entries[id].Entries[child] = new ChildEntry();
            }

            if ((table.Flags & FlagIdentifiers) == 0) return table;

            foreach (var id in ids)
            foreach (var child in members[id])
                table.Entries[id].Entries[child].Identifier = buffer.ReadInt();
            return table;
        }

        public int GetEntryId(string name)
        {
            var hash = name.Hash();
            if (!NamedEntries.ContainsKey(hash))
            {
                return -1;
            }
            return NamedEntries[hash];
        }

        public class ChildEntry
        {
            public int Identifier { get; internal set; } = -1;
        }

        public class Entry
        {
            public int Identifier { get; internal set; } = -1;
            public int Crc { get; internal set; }
            public byte[] Whirlpool { get; } = new byte[64];
            public int Version { get; internal set; }
            public SortedDictionary<int, ChildEntry> Entries { get; } = new SortedDictionary<int, ChildEntry>();
            public int Capacity => Entries.Count == 0 ? 0 : Entries.Keys.Last() + 1;
            public int Size => Entries.Count;
            public ChildEntry GetEntry(int key) => Entries.ContainsKey(key) ? Entries[key] : null;
        }
    }
}