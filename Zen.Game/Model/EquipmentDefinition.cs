using NLog;
using System.Collections.Generic;
using System.IO;
using Zen.Util;

namespace Zen.Game.Model
{
    public class EquipmentDefinition
    {
        public enum WeaponClass
        {
            Axe = InterfaceSet.Interfaces.Axe,
            Maul = InterfaceSet.Interfaces.Maul,
            Bow = InterfaceSet.Interfaces.Bow,
            Claws = InterfaceSet.Interfaces.Claws,
            Longbow = InterfaceSet.Interfaces.Longbow,
            FixedDevice = InterfaceSet.Interfaces.FixedDevice,
            Godsword = InterfaceSet.Interfaces.Godsword,
            Sword = InterfaceSet.Interfaces.Sword,
            Pickaxe = InterfaceSet.Interfaces.Pickaxe,
            Halberd = InterfaceSet.Interfaces.Halberd,
            Staff = InterfaceSet.Interfaces.Staff,
            Scythe = InterfaceSet.Interfaces.Scythe,
            Spear = InterfaceSet.Interfaces.Spear,
            Mace = InterfaceSet.Interfaces.Mace,
            Dagger = InterfaceSet.Interfaces.Dagger,
            MagicStaff = InterfaceSet.Interfaces.MagicStaff,
            Thrown = InterfaceSet.Interfaces.Thrown,
            Unarmed = InterfaceSet.Interfaces.Unarmed,
            Whip = InterfaceSet.Interfaces.Whip
        }

        private static readonly Dictionary<int, EquipmentDefinition> definitions = new Dictionary<int, EquipmentDefinition>();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public const int FlagTwoHanded = 0x1;
        public const int FlagFullHelm = 0x2;
        public const int FlagFullMask = 0x4;
        public const int FlagFullBody = 0x8;

        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public int Slot { get; set; }
        public bool TwoHanded { get; set; }
        public bool FullHelm { get; set; }
        public bool FullMask { get; set; }
        public bool FullBody { get; set; }
        public int Stance { get; set; }
        public WeaponClass Class { get; set; }

        public static void Load()
        {
            using (var reader = new BinaryReader(File.Open(@"../Data/Equipment.dat", FileMode.Open)))
            {
                int id, nextEquipmentId = 0;
                while ((id = reader.ReadShort()) !=  -1)
                {
                    var flags = reader.ReadByte() & 0xFF;
                    var slot = reader.ReadByte() & 0xFF;
                    int stance = 0, weaponClass = 0;
                    if (slot == Equipment.Weapon)
                    {
                        stance = reader.ReadShort() & 0xFFFF;
                        weaponClass = reader.ReadByte() & 0xFF;
                    }

                    var equipment = new EquipmentDefinition
                    {
                        Id = id,
                        EquipmentId = nextEquipmentId++,
                        Slot = slot,
                        TwoHanded = (flags & FlagTwoHanded) != 0,
                        FullHelm = (flags & FlagFullHelm) != 0,
                        FullMask = (flags & FlagFullMask) != 0,
                        FullBody = (flags & FlagFullBody) != 0
                    };

                    if (slot == Equipment.Weapon)
                    {
                        equipment.Stance = stance;
                        equipment.Class = (WeaponClass)weaponClass;
                    }

                    definitions[id] = equipment;
                }

                logger.Info($"Loaded {definitions.Count} equipment definitions");
            }
        }
    }
}
