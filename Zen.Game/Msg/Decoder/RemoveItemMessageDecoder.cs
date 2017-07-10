using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Decoder
{
    public class RemoveItemMessageDecoder : MessageDecoder<RemoveItemMessage>
    {
        public RemoveItemMessageDecoder() : base(81)
        {
        }

        public override RemoveItemMessage Decode(GameFrame frame)
        {
            var reader = new GameFrameReader(frame);

            var itemSlot = (int) reader.GetUnsigned(DataType.Short, transformation: DataTransformation.Add);
            var itemId = (int) reader.GetUnsigned(DataType.Short);
            var inter = (int) reader.GetSigned(DataType.Int, DataOrder.Middle);
            var id = (inter >> 16) & 0xFFFF;
            var slot = inter & 0xFFFF;

            return new RemoveItemMessage(id, slot, itemSlot, itemId);
        }
    }
}