using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Decoder
{
    public class SwapItemsMessageDecoder : MessageDecoder<SwapItemsMessage>
    {
        public SwapItemsMessageDecoder() : base(231)
        {
        }

        public override SwapItemsMessage Decode(GameFrame frame)
        {
            var reader = new GameFrameReader(frame);

            var source = (int) reader.GetUnsigned(DataType.Short);
            var inter = (int) reader.GetSigned(DataType.Int, DataOrder.Little);
            var id = (inter >> 16) & 0xFFFF;
            var slot = inter & 0xFFFF;
            var destination = (int) reader.GetUnsigned(DataType.Short, transformation: DataTransformation.Add);
            var type = (int) reader.GetUnsigned(DataType.Byte, transformation: DataTransformation.Subtract);

            return new SwapItemsMessage(id, slot, source, destination, type);
        }
    }
}