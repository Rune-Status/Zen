using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Decoder
{
    public class DisplayMessageDecoder : MessageDecoder<DisplayMessage>
    {
        public DisplayMessageDecoder() : base(243)
        {
            /* empty */
        }

        public override DisplayMessage Decode(GameFrame frame)
        {
            var reader = new GameFrameReader(frame);

            var mode = (int) reader.GetUnsigned(DataType.Byte);
            var width = (int) reader.GetUnsigned(DataType.Short);
            var height = (int) reader.GetUnsigned(DataType.Short);
            reader.GetUnsigned(DataType.Byte);

            return new DisplayMessage(mode, width, height);
        }
    }
}