using System.Collections.Generic;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NLog;
using Zen.Builder;
using Zen.Game.Msg;

namespace Zen.Net.Game
{
    public class GameMessageDecoder : MessageToMessageDecoder<GameFrame>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly MessageRepository _repository;

        public GameMessageDecoder(MessageRepository repository)
        {
            _repository = repository;
        }

        protected override void Decode(IChannelHandlerContext context, GameFrame message, List<object> output)
        {
            _repository.InCodecs.TryGetValue(message.Opcode, out dynamic decoder);
            if (decoder == null)
            {
                Logger.Debug($"No decoder for packet id {message.Opcode}.");
                return;
            }

            output.Add(decoder.Decode(message));
        }
    }
}