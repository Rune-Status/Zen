using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;
using Zen.Util;

namespace Zen.Game.Update
{
    public class AppearancePlayerBlock : PlayerBlock
    {
        private readonly Appearance _appearance;
        private readonly string _username;

        public AppearancePlayerBlock(Player player) : base(0x4)
        {
            _username = player.Username;
            _appearance = player.Appearance;
        }

        public override void Encode(PlayerUpdateMessage message, GameFrameBuilder builder)
        {
            var gender = _appearance.Gender;
            var propertiesBuilder = new GameFrameBuilder(builder.Allocator);

            var flags = (int) gender;
            propertiesBuilder.Put(DataType.Byte, flags)
                .Put(DataType.Byte, -1)
                .Put(DataType.Byte, -1);

            for (var id = 0; id < 4; id++)
                propertiesBuilder.Put(DataType.Byte, 0);


            propertiesBuilder.Put(DataType.Short, 0x100 | _appearance.Style[2])
                .Put(DataType.Byte, 0)
                .Put(DataType.Short, 0x100 | _appearance.Style[3])
                .Put(DataType.Short, 0x100 | _appearance.Style[5])
                .Put(DataType.Short, 0x100 | _appearance.Style[0])
                .Put(DataType.Short, 0x100 | _appearance.Style[4])
                .Put(DataType.Short, 0x100 | _appearance.Style[6])
                .Put(DataType.Short, 0x100 | _appearance.Style[1]);

            foreach (var color in _appearance.Colors)
                propertiesBuilder.Put(DataType.Byte, color);

            propertiesBuilder.Put(DataType.Short, 1426)
                .Put(DataType.Long, _username.EncodeBase37())
                .Put(DataType.Byte, 138)
                .Put(DataType.Byte, 0)
                .Put(DataType.Byte, 0)
                .Put(DataType.Byte, 0);


            builder.Put(DataType.Byte, DataTransformation.Add, propertiesBuilder.GetLength())
                .PutRawBuilder(propertiesBuilder);
        }
    }
}