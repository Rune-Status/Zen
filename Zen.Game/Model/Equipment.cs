using System;
using Zen.Game.Msg.Impl;
using static Zen.Shared.EquipmentConstants;

namespace Zen.Game.Model
{
    public class Equipment
    {
        public static void Equip(Player player, int slot)
        {
            var inventory = player.Inventory;
            var equipment = player.Equipment;

            var originalWeapon = equipment.Get(Weapon);
            var item = inventory.Get(slot);

            var def = item?.EquipmentDefinition;
            if (def == null) return;

            var targetSlot = def.Slot;

            var unequipShield = def.Slot == Weapon && def.TwoHanded && equipment.Get(Shield) != null;
            var unequipWeapon = targetSlot == Shield
                                && equipment.Get(Weapon) != null
                                && equipment.Get(Weapon).EquipmentDefinition.TwoHanded;
            var topUpStack = item.Definition.Stackable && item.Id == equipment.Get(targetSlot).Id;
            var drainStack = equipment.Get(targetSlot) != null
                             && equipment.Get(targetSlot).Definition.Stackable
                             && inventory.Contains(equipment.Get(targetSlot).Id);

            if ((unequipShield || unequipWeapon) && inventory.FreeSlots == 0)
            {
                inventory.FireCapacityExceeded();
                return;
            }

            if (topUpStack)
            {
                var remaining = equipment.Add(item);
                inventory.Set(slot, remaining);
            }
            else
            {
                if (drainStack)
                {
                    var remaining = inventory.Add(equipment.Get(targetSlot));
                    equipment.Set(targetSlot, remaining);
                    if (remaining != null)
                        return;
                }

                inventory.Remove(item, slot);

                var other = equipment.Get(targetSlot);
                if (other != null)
                    inventory.Add(other);

                equipment.Set(targetSlot, item);
            }

            if (unequipShield)
            {
                var remaining = inventory.Add(equipment.Get(Shield));
                equipment.Set(Shield, remaining);
            }
            else if (unequipWeapon)
            {
                var remaining = inventory.Add(equipment.Get(Weapon));
                equipment.Set(Weapon, remaining);
            }

            var weapon = equipment.Get(Weapon);
            var weaponChanged = false;
            if (originalWeapon == null && weapon != null)
                weaponChanged = true;
            else if (weapon == null && originalWeapon != null)
                weaponChanged = true;
            else if (originalWeapon != null && originalWeapon.Id != weapon.Id)
                weaponChanged = true;

            if (weaponChanged)
                WeaponChanged(player);
        }

        public static void Unequip(Player player, int slot)
        {
            var inventory = player.Inventory;
            var equipment = player.Equipment;

            var item = equipment.Get(slot);
            if (item == null) return;

            var remaining = inventory.Add(item);
            equipment.Set(slot, remaining);


            if (slot == Weapon && remaining == null)
                WeaponChanged(player);
        }

        private static void WeaponChanged(Player player)
        {
            // TODO Attack style setting.
            OpenAttackTab(player);
        }

        public static void OpenAttackTab(Player player)
        {
            var weapon = player.Equipment.Get(Weapon);

            string name;
            EquipmentDefinition.WeaponClass weaponClass;
            if (weapon != null)
            {
                name = weapon.Definition.Name;
                weaponClass = weapon.EquipmentDefinition.Class;
            }
            else
            {
                name = "Unarmed";
                weaponClass = EquipmentDefinition.WeaponClass.Unarmed;
            }

            var tab = (int) weaponClass;
            player.InterfaceSet.OpenTab(InterfaceSet.Tabs.Attack, tab);
            player.Send(new InterfaceTextMessage(tab, 0, name));
        }
    }
}