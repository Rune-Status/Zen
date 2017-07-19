using System.Collections.Generic;
using DotNetty.Buffers;
using NLog;
using Zen.Fs;
using Zen.Util;

namespace Zen.Game.Definition
{
    public class EnumDefinition
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public int KeyType { get; private set; }
        public int ValueType { get; private set; }
        public string DefaultString { get; private set; }
        public int DefaultInt { get; private set; }
        public Dictionary<int, object> Enums { get; } = new Dictionary<int, object>();

        public static EnumDefinition[] Definitions { get; private set; }

        public static void Load(Cache cache)
        {
            var count = 0;

            var container = Container.Decode(cache.Store.Read(255, 17));
            var table = ReferenceTable.Decode(container.Data);

            var files = table.Capacity;
            Definitions = new EnumDefinition[files * 256];

            for (var file = 0; file < files; file++)
            {
                var entry = table.GetEntry(file);
                if (entry == null) continue;

                var archive = Archive.Decode(cache.Read(17, file).Data, entry.Size);
                var nonSparseMember = 0;

                for (var member = 0; member < entry.Capacity; member++)
                {
                    var childEntry = entry.GetEntry(member);
                    if (childEntry == null) continue;

                    var id = file * 256 + member;
                    var definition = Decode(archive.Entries[nonSparseMember++]);
                    Definitions[id] = definition;

                    count++;
                }
            }

            Logger.Info($"Loaded {count} enum definitions.");
        }

        private static EnumDefinition Decode(IByteBuffer buffer)
        {
            var definition = new EnumDefinition();
            int opcode;
            while ((opcode = buffer.ReadByte() & 0xFF) != 0)
                definition.Decode(buffer, opcode);
            return definition;
        }

        private void Decode(IByteBuffer buffer, int opcode)
        {
            if (opcode == 1)
            {
                KeyType = buffer.ReadByte();
            }
            else if (opcode == 2)
            {
                ValueType = buffer.ReadByte();
            }
            else if (opcode == 3)
            {
                DefaultString = buffer.ReadJagexString();
            }
            else if (opcode == 4)
            {
                DefaultInt = buffer.ReadInt();
            }
            else if (opcode == 5 || opcode == 6)
            {
                var count = buffer.ReadUnsignedShort();

                for (var id = 0; id < count; id++)
                {
                    var key = buffer.ReadInt();

                    if (opcode == 5)
                        Enums[key] = buffer.ReadJagexString();
                    else
                        Enums[key] = buffer.ReadInt();
                }
            }
        }

        public int GetInt(int key)
        {
            if (!Enums.ContainsKey(key))
                return DefaultInt;
            return (int) Enums[key];
        }

        public string GetString(int key)
        {
            if (!Enums.ContainsKey(key))
                return DefaultString;
            return (string) Enums[key];
        }

        public static EnumDefinition ForId(int id)
        {
            if (id < 0 || id >= Definitions.Length)
                return null;
            return Definitions[id];
        }
    }
}