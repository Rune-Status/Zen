using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;
using Zen.Util;
using static Zen.Shared.EquipmentConstants;

namespace Zen.Game.Update.Block
{
    public class AppearancePlayerBlock : PlayerBlock
    {
        private readonly Appearance _appearance;
        private readonly int _combatLevel;
        private readonly Container _equipment;
        private readonly int _stance;
        private readonly string _username;

        public AppearancePlayerBlock(Player player) : base(0x4)
        {
            _username = player.Username;
            _appearance = player.Appearance;
            _equipment = player.Equipment;
            _stance = player.Stance;
            _combatLevel = player.SkillSet.GetCombatLevel();
        }

        public override void Encode(PlayerUpdateMessage message, GameFrameBuilder builder)
        {
            var gender = _appearance.Gender;
            var propertiesBuilder = new GameFrameBuilder(builder.Allocator);

            var flags = (int) gender;
            propertiesBuilder.Put(DataType.Byte, flags)
                .Put(DataType.Byte, -1)
                .Put(DataType.Byte, -1);

            Item item;
            for (var slot = 0; slot < 4; slot++)
            {
                item = _equipment.Get(slot);
                if (item != null)
                    propertiesBuilder.Put(DataType.Short, 0x8000 | item.EquipmentDefinition.EquipmentId);
                else
                    propertiesBuilder.Put(DataType.Byte, 0);
            }

            item = _equipment.Get(Body);
            if (item != null)
                propertiesBuilder.Put(DataType.Short, 0x8000 | item.EquipmentDefinition.EquipmentId);
            else
                propertiesBuilder.Put(DataType.Short, 0x100 | _appearance.Style[2]);

            item = _equipment.Get(Shield);
            if (item != null)
                propertiesBuilder.Put(DataType.Short, 0x8000 | item.EquipmentDefinition.EquipmentId);
            else
                propertiesBuilder.Put(DataType.Byte, 0);

            item = _equipment.Get(Body);
            var fullBody = item?.EquipmentDefinition.FullBody ?? false;
            if (!fullBody)
                propertiesBuilder.Put(DataType.Short, 0x100 | _appearance.Style[3]);
            else
                propertiesBuilder.Put(DataType.Byte, 0);

            item = _equipment.Get(Legs);
            if (item != null)
                propertiesBuilder.Put(DataType.Short, 0x8000 | item.EquipmentDefinition.EquipmentId);
            else
                propertiesBuilder.Put(DataType.Short, 0x100 | _appearance.Style[5]);

            item = _equipment.Get(Head);
            var fullHelm = item?.EquipmentDefinition.FullHelm ?? false;
            var fullMask = item?.EquipmentDefinition.FullMask ?? false;
            if (!fullHelm && !fullMask)
                propertiesBuilder.Put(DataType.Short, 0x100 | _appearance.Style[0]);
            else
                propertiesBuilder.Put(DataType.Byte, 0);

            item = _equipment.Get(Hands);
            if (item != null)
                propertiesBuilder.Put(DataType.Short, 0x8000 | item.EquipmentDefinition.EquipmentId);
            else
                propertiesBuilder.Put(DataType.Short, 0x100 | _appearance.Style[4]);

            item = _equipment.Get(Feet);
            if (item != null)
                propertiesBuilder.Put(DataType.Short, 0x8000 | item.EquipmentDefinition.EquipmentId);
            else
                propertiesBuilder.Put(DataType.Short, 0x100 | _appearance.Style[6]);

            item = _equipment.Get(Head);
            if (gender == Gender.Male && !fullMask && !fullHelm)
                propertiesBuilder.Put(DataType.Short, 0x100 | _appearance.Style[1]);
            else
                propertiesBuilder.Put(DataType.Byte, 0);

            foreach (var color in _appearance.Colors)
                propertiesBuilder.Put(DataType.Byte, color);

            propertiesBuilder.Put(DataType.Short, _stance)
                .Put(DataType.Long, _username.EncodeBase37())
                .Put(DataType.Byte, _combatLevel)
                .Put(DataType.Byte, 0)
                .Put(DataType.Byte, 0)
                .Put(DataType.Byte, 0);


            builder.Put(DataType.Byte, DataTransformation.Add, propertiesBuilder.GetLength())
                .PutRawBuilder(propertiesBuilder);
        }
    }
}