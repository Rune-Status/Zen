using System.Collections.Generic;
using DotNetty.Buffers;
using NLog;
using Zen.Fs;
using Zen.Util;

namespace Zen.Game.Definition
{
    public class ObjectDefinition
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ObjectDefinition()
        {
            Name = "null";
            Options = new string[5];
            Varp = -1;
            Impenetrable = true;
            Solid = true;
            Animation = -1;
            Varbit = -1;
        }

        public static ObjectDefinition[] Definitions { get; private set; }
        public Dictionary<int, object> Scripts { get; } = new Dictionary<int, object>();

        public int Varbit { get; private set; }
        public int Animation { get; private set; }
        public bool Solid { get; private set; }
        public int ValidSides { get; private set; }
        public int Varp { get; private set; }
        public string[] Options { get; }
        public string Name { get; private set; }
        public int[] Types { get; private set; }
        public int[] Models { get; private set; }
        public int[] TransformObjects { get; private set; }
        public short[] OriginalTextures { get; private set; }
        public short[] ModifiedTextures { get; private set; }
        public short[] OriginalColors { get; private set; }
        public short[] ModifiedColors { get; private set; }
        public int Length { get; private set; } = 1;
        public int Width { get; private set; } = 1;
        public bool Impenetrable { get; private set; }

        public static void Load(Cache cache)
        {
            var count = 0;

            var container = Container.Decode(cache.Store.Read(255, 16));
            var table = ReferenceTable.Decode(container.Data);

            var files = table.Capacity;
            Definitions = new ObjectDefinition[files * 256];

            for (var file = 0; file < files; file++)
            {
                var entry = table.GetEntry(file);
                if (entry == null) continue;

                var archive = Archive.Decode(cache.Read(16, file).Data, entry.Size);
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

            Logger.Info($"Loaded {count} object definitions.");
        }

        private static ObjectDefinition Decode(IByteBuffer buffer)
        {
            var definition = new ObjectDefinition();
            int opcode;
            while ((opcode = buffer.ReadByte() & 0xFF) != 0)
                definition.Decode(buffer, opcode);
            return definition;
        }

        private void Decode(IByteBuffer buffer, int opcode)
        {
            if (opcode == 1)
            {
                var count = buffer.ReadUnsignedByte();
                if (count <= 0) return;

                Types = new int[count];
                Models = new int[count];

                for (var id = 0; id < count; id++)
                {
                    Models[id] = buffer.ReadUnsignedShort();
                    Types[id] = buffer.ReadUnsignedByte();
                }
            }
            else if (opcode == 2)
            {
                Name = buffer.ReadJagexString();
            }
            else if (opcode == 5)
            {
                var count = buffer.ReadUnsignedByte();
                if (count <= 0) return;
                Models = new int[count];
                Types = null;

                for (var var5 = 0; var5 < count; var5++)
                    Models[var5] = buffer.ReadUnsignedShort();
            }
            else if (opcode == 14)
            {
                Width = buffer.ReadUnsignedByte();
            }
            else if (opcode == 15)
            {
                Length = buffer.ReadUnsignedByte();
            }
            else if (opcode == 17)
            {
                Solid = false;
            }
            else if (opcode == 18)
            {
                Impenetrable = false;
            }
            else if (opcode == 19)
            {
                buffer.ReadUnsignedByte();
            }
            else if (opcode == 24)
            {
                Animation = buffer.ReadUnsignedShort();
                if (Animation == 65535)
                    Animation = -1;
            }
            else if (opcode == 27)
            {
                Impenetrable = false;
            }
            else if (opcode == 28)
            {
                buffer.ReadUnsignedByte();
            }
            else if (opcode == 29)
            {
                buffer.ReadByte();
            }
            else if (opcode == 39)
            {
                buffer.ReadByte();
            }
            else if (opcode >= 30 && opcode < 35)
            {
                Options[opcode - 30] = buffer.ReadJagexString();
            }
            else if (opcode == 40)
            {
                var length = buffer.ReadUnsignedByte();
                OriginalColors = new short[length];
                ModifiedColors = new short[length];

                for (var var5 = 0; var5 < length; var5++)
                {
                    OriginalColors[var5] = (short) buffer.ReadUnsignedShort();
                    ModifiedColors[var5] = (short) buffer.ReadUnsignedShort();
                }
            }
            else if (opcode == 41)
            {
                var length = buffer.ReadUnsignedByte();
                ModifiedTextures = new short[length];
                OriginalTextures = new short[length];

                for (var var5 = 0; var5 < length; var5++)
                {
                    OriginalTextures[var5] = (short) buffer.ReadUnsignedShort();
                    ModifiedTextures[var5] = (short) buffer.ReadUnsignedShort();
                }
            }
            else if (opcode == 42)
            {
                var var4 = buffer.ReadUnsignedByte();
                for (var var5 = 0; var5 < var4; var5++)
                    buffer.ReadByte();
            }
            else if (opcode == 60)
            {
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 65)
            {
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 66)
            {
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 67)
            {
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 69)
            {
                ValidSides = buffer.ReadUnsignedByte();
            }
            else if (opcode == 70)
            {
                buffer.ReadShort();
            }
            else if (opcode == 71)
            {
                buffer.ReadShort();
            }
            else if (opcode == 72)
            {
                buffer.ReadShort();
            }
            else if (opcode == 74)
            {
                Impenetrable = false;
                Solid = false;
            }
            else if (opcode == 75)
            {
                buffer.ReadUnsignedByte();
            }
            else if (opcode == 77 || opcode == 92)
            {
                Varbit = buffer.ReadUnsignedShort();
                if ('\uffff' == Varbit)
                    Varbit = -1;

                Varp = buffer.ReadUnsignedShort();
                if ('\uffff' == Varp)
                    Varp = -1;

                var lastId = -1;
                if (92 == opcode)
                {
                    lastId = buffer.ReadUnsignedShort();
                    if (lastId == '\uffff')
                        lastId = -1;
                }

                var length = buffer.ReadUnsignedByte();
                TransformObjects = new int[length + 2];

                for (var id = 0; length >= id; ++id)
                {
                    TransformObjects[id] = buffer.ReadUnsignedShort();
                    if ('\uffff' == TransformObjects[id])
                        TransformObjects[id] = -1;
                }

                TransformObjects[1 + length] = lastId;
            }
            else if (opcode == 78)
            {
                buffer.ReadUnsignedShort();
                buffer.ReadUnsignedByte();
            }
            else if (opcode == 79)
            {
                buffer.ReadUnsignedShort();
                buffer.ReadUnsignedShort();
                buffer.ReadUnsignedByte();
                var var4 = buffer.ReadUnsignedByte();
                for (var var5 = 0; ~var4 < ~var5; ++var5)
                    buffer.ReadUnsignedShort();
            }
            else if (opcode == 81)
            {
                buffer.ReadUnsignedByte();
            }
           else if (opcode == 93)
            {
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 99)
            {
                buffer.ReadUnsignedByte();
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 100)
            {
                buffer.ReadUnsignedByte();
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 101)
            {
                buffer.ReadUnsignedByte();
            }
            else if (opcode == 102)
            {
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 249)
            {
                var length = buffer.ReadByte() & 0xFF;
                for (var id = 0; id < length; id++)
                {
                    var stringInstance = (buffer.ReadByte() & 0xFF) == 1;
                    var key = buffer.ReadTriByte();

                    if (stringInstance) Scripts[key] = buffer.ReadJagexString();
                    else Scripts[key] = buffer.ReadInt();
                }
            }
        }

        public static ObjectDefinition ForId(int id)
        {
            if (id < 0 || id >= Definitions.Length) return null;
            return Definitions[id];
        }
    }
}