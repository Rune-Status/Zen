using Zen.Game.Msg.Impl;

namespace Zen.Game.Model
{
    public class Equipment : ItemContainer
    {
        public const int Head = 0,
            Cape = 1,
            Neck = 2,
            Weapon = 3,
            Body = 4,
            Shield = 5,
            Legs = 7,
            Hands = 9,
            Feet = 10,
            Ring = 12,
            Ammo = 13;

        public const int Capacity = 14;
        private readonly Player _player;

        public Equipment(Player player) : base(Capacity)
        {
            _player = player;
        }

        public void Unequip(int slot)
        {
            var item = Get(slot);
            if (item == null) return;

            var remaining = _player.Inventory.Add(item);
            Set(slot, remaining);


            if (slot == Weapon && remaining == null)
                WeaponChanged();
        }

        private void WeaponChanged()
        {
        }

        public void OpenAttackTab()
        {
            var weapon = Get(Weapon);

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
            _player.InterfaceSet.OpenTab(InterfaceSet.Tabs.Attack, tab);
            _player.Send(new InterfaceTextMessage(tab, 0, name));
        }

        public override void FireItemChanged(int slot, Item item)
        {
            var items = new[] {new SlottedItem(slot, item)};
            _player.Send(new InterfaceSlottedItemsMessage(387, 28, 94, items));
            _player.Appearance.ResetTicketId();
        }

        public override void FireItemsChanged()
        {
            if (Empty)
            {
                _player.Send(new InterfaceResetItemsMessage(387, 28));
            }
            else
            {
                var items = ToArray();
                _player.Send(new InterfaceItemsMessage(387, 28, 94, items));
            }
            _player.Appearance.ResetTicketId();
        }

        public override void FireCapacityExceeded()
        {
            /* Empty.. this shouldn't be called? */
        }
    }
}