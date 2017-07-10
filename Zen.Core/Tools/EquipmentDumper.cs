using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zen.Fs;
using Zen.Fs.Definition;
using Zen.Game.Model;
using Zen.Shared;
using Zen.Util;
using static Zen.Game.Model.EquipmentDefinition;

namespace Zen.Core.Tools
{
    public class EquipmentDumper
    {
        public static void Dump(Cache cache)
        {
            const string path = GameConstants.WorkingDirectory + "Equipment.json";
            var definitions = new JArray();

            for (var id = 0; id < ItemDefinition.Count; id++)
            {
                var def = ItemDefinition.ForId(id);
                if (def == null) continue;
                if (!IsEquipment(def)) continue;

                dynamic definitionObject = new JObject();

                int flags = 0, slot = GetSlot(def);
                if (IsTwoHanded(def)) flags |= FlagTwoHanded;
                if (IsFullHelm(def)) flags |= FlagFullHelm;
                if (IsFullMask(def)) flags |= FlagFullMask;
                if (IsFullBody(def)) flags |= FlagFullBody;

                definitionObject.Id = id;
                definitionObject.Flags = flags;
                definitionObject.Slot = slot;

                if (slot == Equipment.Weapon)
                {
                    definitionObject.Stance = GetStance(def);
                    definitionObject.WeaponClass = GetWeaponClass(def);
                }

                definitions.Add(definitionObject);
            }

            using (var writer = new JsonTextWriter(new StreamWriter(path)))
            {
                writer.Formatting = Formatting.Indented;
                definitions.WriteTo(writer);
            }


            Console.WriteLine("Successfully dumped equipment data.");
        }

        private static int GetSlot(ItemDefinition def)
        {
            if (def.Name == null) return Equipment.Weapon;
            var name = def.Name.ToLower();

            if (name.Contains("claws")) return Equipment.Weapon;
            if (name.Contains("sword")) return Equipment.Weapon;
            if (name.Contains("dagger")) return Equipment.Weapon;
            if (name.Contains("mace")) return Equipment.Weapon;
            if (name.Contains("whip")) return Equipment.Weapon;
            if (name.Contains("bow")) return Equipment.Weapon;
            if (name.Contains("staff")) return Equipment.Weapon;
            if (name.Contains("dart")) return Equipment.Weapon;

            if (name.Contains("glove")) return Equipment.Hands;
            if (name.Contains("vamb")) return Equipment.Hands;
            if (name.Contains("gaunt")) return Equipment.Hands;

            if (name.Contains("ring")) return Equipment.Ring;
            if (name.Contains("bracelet")) return Equipment.Ring;

            if (name.Contains("amulet")) return Equipment.Neck;
            if (name.Contains("necklace")) return Equipment.Neck;
            if (name.Contains("scarf")) return Equipment.Neck;

            if (name.Contains("leg")) return Equipment.Legs;
            if (name.Contains("bottom")) return Equipment.Legs;
            if (name.Contains("skirt")) return Equipment.Legs;

            if (name.Contains("body")) return Equipment.Body;
            if (name.Contains("top")) return Equipment.Body;
            if (name.Contains("chest")) return Equipment.Body;
            if (name.Contains("chainmail")) return Equipment.Body;

            if (name.Contains("arrow")) return Equipment.Ammo;
            if (name.Contains("bolt")) return Equipment.Ammo;

            if (name.Contains("shield")) return Equipment.Shield;
            if (name.Contains("defender")) return Equipment.Shield;
            if (name.Contains("book")) return Equipment.Shield;

            if (name.Contains("cape")) return Equipment.Cape;
            if (name.Contains("cloak")) return Equipment.Cape;

            if (name.Contains("boot")) return Equipment.Feet;

            if (name.Contains("hat")) return Equipment.Head;
            if (name.Contains("helm")) return Equipment.Head;
            if (name.Contains("mask")) return Equipment.Head;
            if (name.Contains("hood")) return Equipment.Head;

            return name.Contains("coif") ? Equipment.Head : Equipment.Weapon;
        }

        private static bool IsEquipment(ItemDefinition def)
        {
            return def.MaleWearModel1 >= 0 || def.MaleWearModel2 >= 0 || def.FemaleWearModel1 >= 0 ||
                   def.FemaleWearModel2 >= 0;
        }

        private static bool IsTwoHanded(ItemDefinition def)
        {
            if (def.Name == null) return false;
            var name = def.Name.ToLower();

            return name.Contains("2h") || name.Contains(" bow") || name.Contains("godsword") ||
                   name.Contains("claws") || name.Contains("spear") || name.Contains("maul");
        }

        private static bool IsFullBody(ItemDefinition def)
        {
            if (def.Name == null) return false;
            if (GetSlot(def) != Equipment.Body) return false;

            var name = def.Name.ToLower();
            return name.Contains("platebody") || name.Contains("robe");
        }

        private static bool IsFullHelm(ItemDefinition def)
        {
            if (def.Name == null) return false;
            if (GetSlot(def) != Equipment.Head) return false;

            var name = def.Name.ToLower();
            return name.Contains("full");
        }

        private static bool IsFullMask(ItemDefinition def)
        {
            if (def.Name == null) return false;
            if (GetSlot(def) != Equipment.Head) return false;

            var name = def.Name.ToLower();
            return IsFullHelm(def) && name.Contains("mask");
        }

        private static WeaponClass GetWeaponClass(ItemDefinition def)
        {
            if (def.Name == null) return WeaponClass.Sword;
            var name = def.Name.ToLower();

            if (name.Contains("scythe")) return WeaponClass.Scythe;
            if (name.Contains("pickaxe")) return WeaponClass.Pickaxe;
            if (name.Contains("axe")) return WeaponClass.Axe;
            if (name.Contains("godsword")) return WeaponClass.Godsword;
            if (name.Contains("claws")) return WeaponClass.Claws;
            if (name.Contains("longsword")) return WeaponClass.Sword;
            if (name.Contains("scimitar")) return WeaponClass.Sword;
            if (name.Contains("2h sword")) return WeaponClass.Sword;
            if (name.Contains("sword")) return WeaponClass.Dagger;
            if (name.Contains("dagger")) return WeaponClass.Dagger;
            if (name.Contains("mace")) return WeaponClass.Mace;
            if (name.Contains("maul")) return WeaponClass.Maul;
            if (name.Contains("whip")) return WeaponClass.Whip;
            if (name.Contains("longbow")) return WeaponClass.Longbow;
            if (name.Contains("bow")) return WeaponClass.Bow;
            if (name.Contains("staff")) return WeaponClass.Staff;
            if (name.Contains("spear")) return WeaponClass.Spear;

            return name.Contains("dart") ? WeaponClass.Thrown : WeaponClass.Sword;
        }

        private static int GetStance(ItemDefinition def)
        {
            if (def.Name == null) return 1426;

            var name = def.Name.ToLower();
            if (name.Contains("scimitar") || name.Contains("sword"))
                return 1381;
            if (name.Contains("whip"))
                return 620;

            return name.Contains("maul") ? 27 : 1426;
        }
    }
}