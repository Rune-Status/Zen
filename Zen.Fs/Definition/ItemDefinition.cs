using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using NLog;
using Zen.Util;

namespace Zen.Fs.Definition
{
    public class ItemDefinition
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ItemDefinition()
        {
            SecondaryCursorOpcode = -1;
            MaleHead2 = -1;
            PrimaryCursor = -1;
            MaleWearModel2 = -1;
            Ambient = 0;
            FemaleWearModel2 = -1;
            ModelZoom = 2000;
            ResizeZ = 128;
            ResizeY = 128;
            FemaleModelZ = 0;
            MaleModelY = 0;
            Stackable = false;
            ModelOffset1 = 0;
            Name = "null";
            Constrast = 0;
            MaleHead = -1;
            FemaleHead = -1;
            FemaleHead2 = -1;
            ModelRotation1 = 0;
            FemaleWearModel1 = -1;
            LendId = -1;
            MaleWearModel3 = -1;
            LendTemplateId = -1;
            NoteTemplateId = -1;
            MaleModelX = 0;
            FemaleModelX = 0;
            MaleModelZ = 0;
            Unnoted = false;
            NoteId = -1;
            Zangle2D = 0;
            ModelRotation2 = 0;
            InventoryOptions = new[] {null, null, null, null, "Drop"};
            MembersOnly = false;
            MaleWearModel1 = -1;
            TeamId = 0;
            ModelOffset2 = 0;
            PrimaryCursorOpcode = -1;
            FemaleModelY = 0;
            ResizeX = 128;
            SecondaryCursor = -1;
            Value = 1;
            GroundOptions = new[] {null, null, "Take", null, null};
            FemaleWearModel3 = -1;
        }

        public static ItemDefinition[] Definitions { get; private set; }
        public static int Count => Definitions.Length;

        public int SecondaryCursorOpcode { get; private set; }
        public int ResizeY { get; private set; }
        public int MaleModelY { get; private set; }
        public int MaleWearModel2 { get; private set; }
        public int Constrast { get; private set; }
        public int FemaleModelY { get; private set; }
        public byte[] RecolorPalette { get; private set; }
        public int MaleHead2 { get; private set; }
        public int InventoryModelId { get; private set; }
        public int DummyItem { get; private set; }
        public int ModelZoom { get; private set; }
        public int Ambient { get; private set; }
        public int SecondaryCursor { get; private set; }
        public bool Unnoted { get; private set; }
        public int Zangle2D { get; private set; }
        public int FemaleModelZ { get; private set; }
        public int ModelOffset1 { get; private set; }
        public int LendId { get; private set; }
        public int ResizeZ { get; private set; }
        public string Name { get; private set; }
        public short[] ModifiedModelColors { get; private set; }
        public int FemaleWearModel2 { get; private set; }
        public int MaleWearModel3 { get; private set; }
        public int PrimaryCursor { get; private set; }
        public int[] StackableAmounts { get; private set; }
        public short[] OriginalTextureColors { get; private set; }
        public bool Stackable { get; private set; }
        public int LendTemplateId { get; private set; }
        public int MaleHead { get; private set; }
        public int MaleModelZ { get; private set; }
        public short[] OriginalModelColors { get; private set; }
        public int ModelRotation1 { get; private set; }
        public int MaleWearModel1 { get; private set; }
        public int FemaleHead { get; private set; }
        public int MaleModelX { get; private set; }
        public int FemaleHead2 { get; private set; }
        public int PrimaryCursorOpcode { get; private set; }
        public int NoteTemplateId { get; private set; }
        public int FemaleWearModel1 { get; private set; }
        public string[] InventoryOptions { get; private set; }
        public bool MembersOnly { get; private set; }
        public int FemaleModelX { get; private set; }
        public int ResizeX { get; private set; }
        public int ModelRotation2 { get; private set; }
        public int FemaleWearModel3 { get; private set; }
        public short[] ModifiedTextureColors { get; private set; }
        public int ModelOffset2 { get; private set; }
        public int TeamId { get; private set; }
        public int NoteId { get; private set; }
        public string[] GroundOptions { get; private set; }
        public int Value { get; private set; }
        public int[] StackableIds { get; private set; }
        public int PrimaryInterfaceCursorOpcode { get; private set; }
        public int PrimaryInterfaceCursor { get; private set; }
        public int SecondaryInterfaceCursorOpcode { get; private set; }
        public int SecondaryInterfaceCursor { get; private set; }
        public Dictionary<int, object> Scripts { get; private set; } = new Dictionary<int, object>();

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
                    var def = Decode(archive.Entries[nonSparseMember++]);
                    Definitions[id] = def;
                    count++;
                }
            }

            foreach (var definition in Definitions)
                definition?.TransformDefinition();

            Logger.Info($"Loaded {count} item definitions.");
        }

        private void TransformDefinition()
        {
            if (NoteTemplateId != -1)
                ToNoteDefinition(ForId(NoteId), ForId(NoteTemplateId));

            if (LendTemplateId != -1)
                ToLendDefinition(ForId(LendId), ForId(LendTemplateId));
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
            }
            else if (opcode == 8)
            {
                ModelOffset2 = buffer.ReadUnsignedShort();
                if (ModelOffset2 > 32767)
                    ModelOffset2 -= 65536;
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
            else if (opcode == 23)
            {
                MaleWearModel1 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 24)
            {
                MaleWearModel2 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 25)
            {
                FemaleWearModel1 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 26)
            {
                FemaleWearModel2 = buffer.ReadUnsignedShort();
            }
            else if (opcode >= 30 && opcode < 35)
            {
                GroundOptions[opcode - 30] = buffer.ReadString();
                if (GroundOptions[opcode - 30].Equals("Hidden", StringComparison.InvariantCultureIgnoreCase))
                    GroundOptions[opcode - 30] = null;
            }
            else if (opcode >= 35 && opcode < 40)
            {
                InventoryOptions[opcode - 35] = buffer.ReadString();
            }
            else if (opcode == 40)
            {
                var length = buffer.ReadByte() & 0xFF;
                OriginalModelColors = new short[length];
                ModifiedModelColors = new short[length];
                for (var id = 0; id < length; id++)
                {
                    OriginalModelColors[id] = (short) buffer.ReadUnsignedShort();
                    ModifiedModelColors[id] = (short) buffer.ReadUnsignedShort();
                }
            }
            else if (opcode == 41)
            {
                var length = buffer.ReadByte() & 0xFF;
                OriginalTextureColors = new short[length];
                ModifiedTextureColors = new short[length];
                for (var id = 0; id < length; id++)
                {
                    OriginalTextureColors[id] = (short) buffer.ReadUnsignedShort();
                    ModifiedTextureColors[id] = (short) buffer.ReadUnsignedShort();
                }
            }
            else if (opcode == 42)
            {
                var length = buffer.ReadByte() & 0xFF;
                RecolorPalette = new byte[length];
                for (var id = 0; id < length; id++)
                    RecolorPalette[id] = buffer.ReadByte();
            }
            else if (opcode == 65)
            {
                Unnoted = true;
            }
            else if (opcode == 78)
            {
                MaleWearModel3 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 79)
            {
                FemaleWearModel3 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 90)
            {
                MaleHead = buffer.ReadUnsignedShort();
            }
            else if (opcode == 91)
            {
                FemaleHead = buffer.ReadUnsignedShort();
            }
            else if (opcode == 92)
            {
                MaleHead2 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 93)
            {
                FemaleHead2 = buffer.ReadUnsignedShort();
            }
            else if (opcode == 95)
            {
                Zangle2D = buffer.ReadUnsignedShort();
            }
            else if (opcode == 96)
            {
                DummyItem = buffer.ReadByte() & 0xFF;
            }
            else if (opcode == 97)
            {
                NoteId = buffer.ReadUnsignedShort();
            }
            else if (opcode == 98)
            {
                NoteTemplateId = buffer.ReadUnsignedShort();
            }
            else if (opcode >= 100 && opcode < 110)
            {
                if (StackableIds == null)
                {
                    StackableAmounts = new int[10];
                    StackableIds = new int[10];
                }
                StackableIds[opcode - 100] = buffer.ReadUnsignedShort();
                StackableAmounts[opcode - 100] = buffer.ReadUnsignedShort();
            }
            else if (opcode == 110)
            {
                ResizeX = buffer.ReadUnsignedShort();
            }
            else if (opcode == 111)
            {
                ResizeY = buffer.ReadUnsignedShort();
            }
            else if (opcode == 112)
            {
                ResizeZ = buffer.ReadUnsignedShort();
            }
            else if (opcode == 113)
            {
                Ambient = buffer.ReadByte();
            }
            else if (opcode == 114)
            {
                Constrast = buffer.ReadByte() * 5;
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
            else if (opcode == 125)
            {
                MaleModelX = buffer.ReadByte();
                MaleModelY = buffer.ReadByte();
                MaleModelZ = buffer.ReadByte();
            }
            else if (opcode == 126)
            {
                FemaleModelX = buffer.ReadByte();
                FemaleModelY = buffer.ReadByte();
                FemaleModelZ = buffer.ReadByte();
            }
            else if (opcode == 127)
            {
                PrimaryCursorOpcode = buffer.ReadByte() & 0xFF;
                PrimaryCursor = buffer.ReadUnsignedShort();
            }
            else if (opcode == 128)
            {
                SecondaryCursorOpcode = buffer.ReadByte() & 0xFF;
                SecondaryCursor = buffer.ReadUnsignedShort();
            }
            else if (opcode == 129)
            {
                PrimaryInterfaceCursorOpcode = buffer.ReadByte() & 0xFF;
                PrimaryInterfaceCursor = buffer.ReadUnsignedShort();
            }
            else if (opcode == 130)
            {
                SecondaryInterfaceCursorOpcode = buffer.ReadByte() & 0xFF;
                SecondaryInterfaceCursor = buffer.ReadUnsignedShort();
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

        private void ToNoteDefinition(ItemDefinition link, ItemDefinition template)
        {
            Value = link.Value;
            RecolorPalette = template.RecolorPalette;
            ModifiedModelColors = template.ModifiedModelColors;
            OriginalTextureColors = template.OriginalTextureColors;
            ModelRotation2 = template.ModelRotation2;
            ModelRotation1 = template.ModelRotation1;
            ModelOffset1 = template.ModelOffset1;
            OriginalModelColors = template.OriginalModelColors;
            InventoryModelId = template.InventoryModelId;
            Name = link.Name;
            Stackable = true;
            Zangle2D = template.Zangle2D;
            MembersOnly = link.MembersOnly;
            ModelOffset2 = template.ModelOffset2;
            ModelZoom = template.ModelZoom;
            ModifiedTextureColors = template.ModifiedTextureColors;
        }

        private void ToLendDefinition(ItemDefinition link, ItemDefinition template)
        {
            FemaleWearModel1 = link.FemaleWearModel1;
            MaleHead2 = link.MaleHead2;
            MaleHead = link.MaleHead;
            FemaleWearModel2 = link.FemaleWearModel2;
            MaleWearModel3 = link.MaleWearModel3;
            ModelOffset1 = template.ModelOffset1;
            FemaleModelY = link.FemaleModelY;
            MaleWearModel2 = link.MaleWearModel2;
            Scripts = link.Scripts;
            OriginalModelColors = link.OriginalModelColors;
            Zangle2D = template.Zangle2D;
            FemaleHead2 = link.FemaleHead2;
            MaleModelY = link.MaleModelY;
            MaleModelZ = link.MaleModelZ;
            TeamId = link.TeamId;
            ModelRotation2 = template.ModelRotation2;
            MaleModelX = link.MaleModelX;
            InventoryModelId = template.InventoryModelId;
            InventoryOptions = new string[5];
            MembersOnly = link.MembersOnly;
            ModifiedModelColors = link.ModifiedModelColors;
            FemaleModelZ = link.FemaleModelZ;
            ModelOffset2 = template.ModelOffset2;
            ModifiedTextureColors = link.ModifiedTextureColors;
            ModelRotation1 = template.ModelRotation1;
            MaleWearModel1 = link.MaleWearModel1;
            OriginalTextureColors = link.OriginalTextureColors;
            FemaleModelX = link.FemaleModelX;
            FemaleHead = link.FemaleHead;
            ModelZoom = template.ModelZoom;
            FemaleWearModel3 = link.FemaleWearModel3;
            GroundOptions = link.GroundOptions;
            Name = link.Name;
            Value = 0;
            RecolorPalette = link.RecolorPalette;
            if (link.InventoryOptions != null)
                for (var option = 0; option < 4; option++)
                    InventoryOptions[option] = link.InventoryOptions[option];
            InventoryOptions[4] = "Take";
        }

        public static ItemDefinition ForId(int id)
        {
            if (id < 0 || id >= Definitions.Length)
                return null;
            return Definitions[id];
        }
    }
}