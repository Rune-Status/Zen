using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.IO;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class RegionChangeMessageEncoder : MessageEncoder<RegionChangeMessage>
    {
        private readonly LandscapeKeyTable _keyTable;

        public RegionChangeMessageEncoder(LandscapeKeyTable keyTable)
        {
            _keyTable = keyTable;
        }

        public override GameFrame Encode(IByteBufferAllocator alloc, RegionChangeMessage message)
        {
            var position = message.Position;
            var builder = new GameFrameBuilder(alloc, 162, FrameType.VariableShort)
                .Put(DataType.Short, DataTransformation.Add, position.GetLocalX(position.CentralRegionX));

            var force = true;
            var centralMapX = position.CentralRegionX / 8;
            var centralMapY = position.CentralRegionY / 8;

            if ((centralMapX == 48 || centralMapX == 49) && centralMapY == 48)
                force = false;

            if (centralMapX == 48 && centralMapY == 148)
                force = false;

            for (var mapX = (position.CentralRegionX - 6) / 8; mapX <= (position.CentralRegionX + 6) / 8; mapX++)
            for (var mapY = (position.CentralRegionY - 6) / 8; mapY <= (position.CentralRegionY + 6) / 8; mapY++)
            {
                if (!force && (mapY == 49 || mapY == 149 || mapY == 147 || mapX == 50 ||
                               mapX == 49 && mapY == 47)) continue;

                var keys = _keyTable.GetKeys(mapX, mapY);
                foreach (var key in keys)
                    builder.Put(DataType.Int, DataOrder.InversedMiddle, key);
            }

            return builder.Put(DataType.Byte, DataTransformation.Subtract, position.Height)
                .Put(DataType.Short, position.CentralRegionX)
                .Put(DataType.Short, DataTransformation.Add, position.CentralRegionY)
                .Put(DataType.Short, DataTransformation.Add, position.GetLocalY(position.CentralRegionY))
                .ToGameFrame();
        }
    }
}