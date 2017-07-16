using System.Collections.Generic;
using DotNetty.Buffers;
using NLog;
using Zen.Fs;
using Zen.Util;

namespace Zen.Game.Definition
{
    public class ItemDefinition
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static ItemDefinition[] Definitions { get; private set; }

        public int InventoryModelId { get; private set; }
        public string Name { get; private set; }
        public int ModelOffset1 { get; private set; }
        public int ModelRotation2 { get; private set; }
        public int ModelRotation1 { get; private set; }
        public int ModelZoom { get; private set; }
        public int LendTemplateId { get; private set; } = -1;
        public int LendId { get; private set; } = -1;
        public int TeamId { get; private set; }
        public int[] StackableAmounts { get; } = new int[10];
        public int[] StackableIds { get; } = new int[10];
        public int NotedTemplateId { get; private set; } = -1;
        public int NotedId { get; private set; } = -1;
        public int ColorEquip2 { get; private set; }
        public int ColorEquip1 { get; private set; }
        public bool Noted { get; private set; }
        public int[] ModifiedTextureColor { get; private set; }
        public int[] OriginalTextureColor { get; private set; }
        public int[] ModifiedModelColors { get; private set; }
        public int[] OriginalModelColors { get; private set; }
        public string[] InventoryOptions { get; private set; } = {null, null, "take", null, null};
        public string[] GroundOptions { get; private set; } = {null, null, null, null, "drop"};
        public int FemaleWearModel2 { get; private set; } = -1;
        public int MaleWearModel2 { get; private set; } = -1;
        public int FemaleWearModel1 { get; private set; } = -1;
        public int MaleWearModel1 { get; private set; } = -1;
        public bool MembersOnly { get; private set; }
        public int Value { get; private set; }
        public bool Stackable { get; private set; }
        public int ModelOffset2 { get; private set; }
        public Dictionary<int, object> Scripts { get; private set; } = new Dictionary<int, object>();

        public static int Count => Definitions.Length;

        public static void Load(Cache cache)
        {
            var count = 0;

            var container = Container.Decode(cache.Store.Read(255, 19));
            var table = ReferenceTable.Decode(container.Data);

            var files = table.Capacity;
            Definitions = new ItemDefinition[files * 256];

            for (var file = 0; file < files; file++)
            {
                var entry = table.GetEntry(file);
                if (entry == null) continue;

                var archive = Archive.Decode(cache.Read(19, file).Data, entry.Size);
                var nonSparseMember = 0;

                for (var member = 0; member < entry.Capacity; member++)
                {
                    var childEntry = entry.GetEntry(member);
                    if (childEntry == null) continue;

                    var id = file * 256 + member;
                    var definition = Decode(archive.Entries[nonSparseMember++]);

                    if (definition.NotedTemplateId != -1) definition.ToNoteTemplate();
                    if (definition.LendTemplateId != -1) definition.ToLendTemplate();

                    Definitions[id] = definition;
                    count++;
                }
            }
            Logger.Info($"Loaded {count} item definitions.");
        }

        private void ToLendTemplate()
        {
            var definition = ForId(LendId);
            OriginalModelColors = definition.OriginalModelColors;
            ColorEquip1 = definition.ColorEquip1;
            ColorEquip2 = definition.ColorEquip2;
            TeamId = definition.TeamId;
            Value = 0;
            MembersOnly = definition.MembersOnly;
            Name = definition.Name;
            InventoryOptions = new string[5];
            GroundOptions = definition.GroundOptions;
            if (definition.InventoryOptions != null)
                for (var id = 0; id < 4; id++)
                    InventoryOptions[id] = definition.InventoryOptions[id];
            InventoryOptions[4] = "Discard";
            MaleWearModel1 = definition.MaleWearModel1;
            MaleWearModel2 = definition.MaleWearModel2;
            FemaleWearModel1 = definition.FemaleWearModel1;
            FemaleWearModel2 = definition.FemaleWearModel2;
            Scripts = definition.Scripts;
        }

        private void ToNoteTemplate()
        {
            var definition = ForId(NotedId);
            MembersOnly = definition.MembersOnly;
            Value = definition.Value;
            Name = definition.Name;
            Stackable = definition.Stackable;
            Noted = true;
        }

        private static ItemDefinition Decode(IByteBuffer buffer)
        {
            var definition = new ItemDefinition();
            int opcode;
            while ((opcode = buffer.ReadByte() & 0xFF) != 0)
                definition.Decode(buffer, opcode);
            return definition;
        }

        private void Decode(IByteBuffer buffer, int opcode)
        {
            if (opcode == 1)
            {
                InventoryModelId = buffer.ReadUnsignedShort();
            }
            else if (opcode == 2)
            {
                Name = buffer.ReadJagexString();
            }
            else if (opcode == 4)
            {
                ModelZoom = buffer.ReadUnsignedShort();
            }
            else if (opcode == 5)
            {
                ModelRotation1 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 6)
            {
                ModelRotation2 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 7)
            {
                ModelOffset1 = buffer.ReadUnsignedShort();
                if (ModelOffset1 > 32767)
                    ModelOffset1 -= 65536;
                ModelOffset1 <<= 0;
            }
            else if (opcode == 8)
            {
                ModelOffset2 = buffer.ReadUnsignedShort();
                if (ModelOffset2 > 32767)
                    ModelOffset2 -= 65536;
                ModelOffset2 <<= 0;
            }
            else if (opcode == 11)
            {
                Stackable = true;
            }
            else if (opcode == 12)
            {
                Value = buffer.ReadInt();
            }
            else if (opcode == 16)
            {
                MembersOnly = true;
            }
            else if (opcode == 18)
            {
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 23)
            {
                MaleWearModel1 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 24)
            {
                FemaleWearModel1 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 25)
            {
                MaleWearModel2 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 26)
            {
                FemaleWearModel2 = buffer.ReadUnsignedShort();
            }
            else if (opcode >= 30 && opcode < 35)
            {
                GroundOptions[opcode - 30] = buffer.ReadJagexString();
            }
            else if (opcode >= 35 && opcode < 40)
            {
                InventoryOptions[opcode - 35] = buffer.ReadJagexString();
            }
            else if (opcode == 40)
            {
                var length = buffer.ReadByte() & 0xFF;

                OriginalModelColors = new int[length];
                ModifiedModelColors = new int[length];

                for (var id = 0; id < length; id++)
                {
                    OriginalModelColors[id] = buffer.ReadUnsignedShort();
                    ModifiedModelColors[id] = buffer.ReadUnsignedShort();
                }
            }
            else if (opcode == 41)
            {
                var length = buffer.ReadByte() & 0xFF;

                OriginalTextureColor = new int[length];
                ModifiedTextureColor = new int[length];

                for (var id = 0; id < length; id++)
                {
                    OriginalTextureColor[id] = buffer.ReadUnsignedShort();
                    ModifiedTextureColor[id] = buffer.ReadUnsignedShort();
                }
            }
            else if (opcode == 42)
            {
                var length = buffer.ReadByte() & 0xFF;
                for (var id = 0; id < length; id++) buffer.ReadByte();
            }
            else if (opcode == 65)
            {
                Noted = false;
            }
            else if (opcode == 78)
            {
                ColorEquip1 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 79)
            {
                ColorEquip2 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 90 || opcode == 91 || opcode == 92 || opcode == 93 || opcode == 95 || opcode == 110 ||
                     opcode == 111 || opcode == 112)
            {
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 96 || opcode == 113 || opcode == 114)
            {
                buffer.ReadByte();
            }
            else if (opcode == 97)
            {
                NotedId = buffer.ReadUnsignedShort();
            }
            else if (opcode == 98)
            {
                NotedTemplateId = buffer.ReadUnsignedShort();
            }
            else if (opcode >= 100 && opcode < 110)
            {
                StackableIds[opcode - 100] = buffer.ReadUnsignedShort();
                StackableAmounts[opcode - 100] = buffer.ReadUnsignedShort();
            }
            else if (opcode == 115)
            {
                TeamId = buffer.ReadByte() & 0xFF;
            }
            else if (opcode == 121)
            {
                LendId = buffer.ReadUnsignedShort();
            }
            else if (opcode == 122)
            {
                LendTemplateId = buffer.ReadUnsignedShort();
            }
            else if (opcode == 125 || opcode == 126)
            {
                for (var i = 0; i < 3; i++) buffer.ReadByte();
            }
            else if (opcode == 127 || opcode == 128 || opcode == 129 || opcode == 130)
            {
                buffer.ReadByte();
                buffer.ReadUnsignedShort();
            }
            else if (opcode == 132)
            {
                var length = buffer.ReadByte() & 0xFF;
                for (var id = 0; id < length; id++) buffer.ReadUnsignedShort();
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

        public static ItemDefinition ForId(int id)
        {
            if (id < 0 || id >= Definitions.Length)
                return null;
            return Definitions[id];
        }
    }
}