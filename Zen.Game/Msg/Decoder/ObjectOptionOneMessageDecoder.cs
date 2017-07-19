using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Decoder
{
    public class ObjectOptionOneMessageDecoder : MessageDecoder<ObjectOptionOneMessage>
    {
        public ObjectOptionOneMessageDecoder() : base(254)
        {
        }

        public override ObjectOptionOneMessage Decode(GameFrame frame)
        {
            var reader = new GameFrameReader(frame);

            var x = (int) reader.GetSigned(DataType.Short, DataOrder.Little);
            var id = (int) reader.GetSigned(DataType.Short, transformation: DataTransformation.Add);
            var y = (int) reader.GetSigned(DataType.Short);

            return new ObjectOptionOneMessage(x, y, id);
        }
    }
}