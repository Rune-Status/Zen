using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zen.Fs;
using Zen.Fs.Definition;
using Zen.Shared;
using Zen.Util;
using static Zen.Game.Model.EquipmentDefinition;
using static Zen.Shared.EquipmentConstants;

namespace Zen.Core.Tools
{
    public class EquipmentDumper
    {
        public static void Main()
        {
            var cache = new Cache(FileStore.Open(GameConstants.CacheDirectory));
            ItemDefinition.Load(cache);

            const string path = GameConstants.WorkingDirectory + "Equipment.json";
            var definitions = new JArray();

            var stances = new Dictionary<int, int>();
            using (var reader = new StreamReader(GameConstants.WorkingDirectory + "Misc/Stances.txt"))
            {
                var id = 0;
                string line;

                while ((line = reader.ReadLine()) != null)
                    if (line.Contains("itemId:"))
                    {
                        id = int.Parse(line.Substring(8));
                    }
                    else if (line.Contains("renderEmote"))
                    {
                        var animation = int.Parse(line.Substring(12));
                        stances[id] = animation;
                    }
            }

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

                if (slot == Weapon)
                {
                    definitionObject.Stance = stances.ContainsKey(id) ? stances[id] : 1426;
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
            Console.ReadLine();
        }

        private static int GetSlot(ItemDefinition def)
        {
            if (def.Name == null) return Weapon;
            var name = def.Name.ToLower();

            if (name.Contains("claws")) return Weapon;
            if (name.Contains("sword")) return Weapon;
            if (name.Contains("dagger")) return Weapon;
            if (name.Contains("mace")) return Weapon;
            if (name.Contains("whip")) return Weapon;
            if (name.Contains("bow")) return Weapon;
            if (name.Contains("staff")) return Weapon;
            if (name.Contains("dart")) return Weapon;

            if (name.Contains("glove")) return Hands;
            if (name.Contains("vamb")) return Hands;
            if (name.Contains("gaunt")) return Hands;

            if (name.Contains("ring")) return Ring;
            if (name.Contains("bracelet")) return Ring;

            if (name.Contains("amulet")) return Neck;
            if (name.Contains("necklace")) return Neck;
            if (name.Contains("scarf")) return Neck;

            if (name.Contains("leg")) return Legs;
            if (name.Contains("bottom")) return Legs;
            if (name.Contains("skirt")) return Legs;

            if (name.Contains("body")) return Body;
            if (name.Contains("top")) return Body;
            if (name.Contains("chest")) return Body;
            if (name.Contains("chainmail")) return Body;

            if (name.Contains("arrow")) return Ammo;
            if (name.Contains("bolt")) return Ammo;

            if (name.Contains("shield")) return Shield;
            if (name.Contains("defender")) return Shield;
            if (name.Contains("book")) return Shield;

            if (name.Contains("cape")) return Cape;
            if (name.Contains("cloak")) return Cape;

            if (name.Contains("boot")) return Feet;

            if (name.Contains("hat")) return Head;
            if (name.Contains("helm")) return Head;
            if (name.Contains("mask")) return Head;
            if (name.Contains("hood")) return Head;

            return name.Contains("coif") ? Head : Weapon;
        }

        private static bool IsEquipment(ItemDefinition def)
        {
            if (def.Name != null && def.Name.ContainsWord("Ring"))
                return true;

            return def.MaleWearModel1 >= 0 || def.MaleWearModel2 >= 0 ||
                   def.FemaleWearModel1 >= 0 ||
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
            if (GetSlot(def) != Body) return false;

            var name = def.Name.ToLower();
            return name.Contains("platebody") || name.Contains("robe") || name.Contains("chestplate");
        }

        private static bool IsFullHelm(ItemDefinition def)
        {
            if (def.Name == null) return false;
            if (GetSlot(def) != Head) return false;

            var name = def.Name.ToLower();
            return name.Contains("full");
        }

        private static bool IsFullMask(ItemDefinition def)
        {
            if (def.Name == null) return false;
            if (GetSlot(def) != Head) return false;

            var name = def.Name.ToLower();
            return IsFullHelm(def) || name.Contains("mask") || name.Equals("slayer helmet");
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
    }
}