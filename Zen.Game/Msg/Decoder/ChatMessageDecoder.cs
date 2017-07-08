using Zen.Builder;
using Zen.Game.Msg.Impl;
using Zen.Util;

namespace Zen.Game.Msg.Decoder
{
    public class ChatMessageDecoder : MessageDecoder<ChatMessage>
    {
        public ChatMessageDecoder() : base(237)
        {
        }

        public override ChatMessage Decode(GameFrame frame)
        {
            var reader = new GameFrameReader(frame);
            var size = reader.GetLength() - 2;

            var color = (int) reader.GetUnsigned(DataType.Byte);
            var effects = (int) reader.GetUnsigned(DataType.Byte);

            var bytes = new byte[size];
            reader.GetBytes(bytes);
            var text = bytes.Unpack();

            return new ChatMessage(color, effects, text);
        }
    }
}