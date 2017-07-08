using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Decoder
{
    public class ButtonMessageDecoder : MessageDecoder<ButtonMessage>
    {
        public ButtonMessageDecoder(int opcode) : base(opcode)
        {
        }

        public override ButtonMessage Decode(GameFrame frame)
        {
            var reader = new GameFrameReader(frame);

            var button = (int) reader.GetSigned(DataType.Int);
            var id = (button >> 16) & 0xFFFF;
            var slot = button & 0xFFFF;
            var parameter = -1;
            if (Opcode == 155)
                parameter = (int) reader.GetUnsigned(DataType.Short);

            return new ButtonMessage(id, slot, parameter);
        }
    }
}