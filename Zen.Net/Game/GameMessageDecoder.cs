using System.Collections.Generic;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Zen.Builder;
using Zen.Game.Msg;

namespace Zen.Net.Game
{
    public class GameMessageDecoder : MessageToMessageDecoder<GameFrame>
    {
        private readonly MessageRepository _repository;

        public GameMessageDecoder(MessageRepository repository)
        {
            _repository = repository;
        }

        protected override void Decode(IChannelHandlerContext context, GameFrame message, List<object> output)
        {
            _repository.InCodecs.TryGetValue(message.Opcode, out dynamic decoder);
            if (decoder == null)
                return;

            output.Add(decoder.Decode(message));
        }
    }
}