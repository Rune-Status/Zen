using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Decoder
{
    public class CommandMessageDecoder : MessageDecoder<CommandMessage>
    {
        public CommandMessageDecoder() : base(44)
        {
        }

        public override CommandMessage Decode(GameFrame frame)
        {
            var reader = new GameFrameReader(frame);
            var keyword = reader.ReadString();

            return new CommandMessage(keyword);
        }
    }
}