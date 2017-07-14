using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Zen.Fs.Definition;
using Zen.Shared;
using Zen.Util;

namespace Zen.Game.Model
{
    public class EquipmentDefinition
    {
        public enum WeaponClass
        {
            Axe = Interfaces.Axe,
            Maul = Interfaces.Maul,
            Bow = Interfaces.Bow,
            Claws = Interfaces.Claws,
            Longbow = Interfaces.Longbow,
            FixedDevice = Interfaces.FixedDevice,
            Godsword = Interfaces.Godsword,
            Sword = Interfaces.Sword,
            Pickaxe = Interfaces.Pickaxe,
            Halberd = Interfaces.Halberd,
            Staff = Interfaces.Staff,
            Scythe = Interfaces.Scythe,
            Spear = Interfaces.Spear,
            Mace = Interfaces.Mace,
            Dagger = Interfaces.Dagger,
            MagicStaff = Interfaces.MagicStaff,
            Thrown = Interfaces.Thrown,
            Unarmed = Interfaces.Unarmed,
            Whip = Interfaces.Whip
        }

        public const int FlagTwoHanded = 0x1;
        public const int FlagFullHelm = 0x2;
        public const int FlagFullMask = 0x4;
        public const int FlagFullBody = 0x8;

        private static readonly Dictionary<int, EquipmentDefinition> Definitions =
            new Dictionary<int, EquipmentDefinition>();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public EquipmentDefinition(int equipmentId, int slot, bool twoHanded, bool fullHelm, bool fullMask,
            bool fullBody, int stance, WeaponClass weaponClass)
        {
            EquipmentId = equipmentId;
            Slot = slot;
            TwoHanded = twoHanded;
            FullHelm = fullHelm;
            FullMask = fullMask;
            FullBody = fullBody;
            Stance = stance;
            Class = weaponClass;
        }

        public int EquipmentId { get; }
        public int Slot { get; }
        public bool TwoHanded { get; }
        public bool FullHelm { get; }
        public bool FullMask { get; }
        public bool FullBody { get; }
        public int Stance { get; }
        public WeaponClass Class { get; }

        public static void Load()
        {
            const string path = GameConstants.WorkingDirectory + "Equipment.json";
            if (!File.Exists(path)) throw new FileNotFoundException();

            JArray definitions;
            using (var reader = new JsonTextReader(File.OpenText(path)))
            {
                definitions = JToken.ReadFrom(reader) as JArray;
            }

            if (definitions == null) throw new ArgumentException();
            var nextEquipmentId = 0;

            foreach (dynamic definition in definitions)
            {
                int id = definition.Id;
                int flags = definition.Flags;
                int slot = definition.Slot;

                int stance = 0, weaponClass = 0;
                if (slot == EquipmentConstants.Weapon)
                {
                    stance = definition.Stance;
                    weaponClass = definition.WeaponClass;
                }

                var twoHanded = (flags & FlagTwoHanded) != 0;
                var fullHelm = (flags & FlagFullHelm) != 0;
                var fullMask = (flags & FlagFullMask) != 0;
                var fullBody = (flags & FlagFullBody) != 0;

                var invisible = ItemDefinition.ForId(id).Name.ContainsWord("Ring");
                Definitions[id] = new EquipmentDefinition(invisible ? -1 : nextEquipmentId++, slot, twoHanded, fullHelm,
                    fullMask,
                    fullBody, stance,
                    (WeaponClass) weaponClass);
            }

            Logger.Info($"Loaded {Definitions.Count} item definitions.");
        }

        public static EquipmentDefinition ForId(int id)
        {
            if (id < 0 || !Definitions.ContainsKey(id)) return null;
            return Definitions[id];
        }
    }
}