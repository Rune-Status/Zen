using System;
using System.IO;
using Zen.Fs;
using Zen.Fs.Definition;
using Zen.Game.Model;

namespace Zen.Core.Tools
{
    public class EquipmentDumper
    {
        public static void Dump(Cache cache)
        {
            using (var writer = new BinaryWriter(File.Open(@"../Data/Equipment.bin", FileMode.Create)))
            {
                for (var id = 0; id < ItemDefinition.Count; id++)
                {
                    var def = ItemDefinition.ForId(id);
                    if (def == null) continue;
                    else if (!IsEquipment(def)) continue;

                    WriteShort(writer, id);

                    int flags = 0, slot = GetSlot(def);
                }
            }
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
            if (name.Contains("coif")) return Equipment.Head;

            return Equipment.Weapon;
        }

        private static bool IsEquipment(ItemDefinition def)
        {
            return def.MaleWearModel1 >= 0 || def.MaleWearModel2 >= 0 || def.FemaleWearModel1 >= 0 ||
                   def.FemaleWearModel2 >= 0;
        }

        public static void WriteByte(BinaryWriter writer, int value) => writer.BaseStream.WriteByte((byte) value);

        public static void WriteShort(BinaryWriter writer, int value)
        {
            WriteByte(writer, value >> 8);
            WriteByte(writer, value);
        }
    }
}