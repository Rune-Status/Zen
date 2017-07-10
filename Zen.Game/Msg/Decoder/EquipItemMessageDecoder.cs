using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Decoder
{
    public class EquipItemMessageDecoder : MessageDecoder<EquipItemMessage>
    {
        public EquipItemMessageDecoder() : base(55)
        {
        }

        public override EquipItemMessage Decode(GameFrame frame)
        {
            var reader = new GameFrameReader(frame);

            var itemId = (int) reader.GetUnsigned(DataType.Short, DataOrder.Little);
            var itemSlot = (int) reader.GetUnsigned(DataType.Short, transformation: DataTransformation.Add);
            var inter = (int) reader.GetSigned(DataType.Int, DataOrder.Middle);
            var id = (inter >> 16) & 0xFFFF;
            var slot = inter & 0xFFFF;

            return new EquipItemMessage(id, slot, itemSlot, itemId);
        }
    }
}