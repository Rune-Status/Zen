using System.Collections.Generic;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Zen.Builder;
using Zen.Game.Msg;

namespace Zen.Net.Game
{
    public class GameMessageEncoder : MessageToMessageEncoder<IMessage>
    {
        private readonly MessageRepository _repository;

        public GameMessageEncoder(MessageRepository repository)
        {
            _repository = repository;
        }

        protected override void Encode(IChannelHandlerContext context, IMessage message, List<object> output)
        {
            _repository.OutCodecs.TryGetValue(message.GetType(), out dynamic encoder);
            if (encoder == null) return;

            GameFrame frame = encoder.Encode(context.Allocator, (dynamic) message);
            output.Add(frame);
        }
    }
}