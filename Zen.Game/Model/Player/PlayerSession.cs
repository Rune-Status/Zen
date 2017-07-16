using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Zen.Game.Msg;

namespace Zen.Game.Model.Player
{
    public class PlayerSession
    {
        private readonly IChannel _channel;
        private readonly Queue<IMessage> _messages = new Queue<IMessage>();

        private readonly Player _player;
        private readonly MessageRepository _repository;

        public PlayerSession(Player player, IChannel channel, MessageRepository repository)
        {
            _player = player;
            _channel = channel;
            _repository = repository;
        }

        public void Enqueue(IMessage msg)
        {
            lock (_messages)
            {
                _messages.Enqueue(msg);
            }
        }

        public void ProcessMessageQueue()
        {
            lock (_messages)
            {
                IMessage message;
                while (_messages.Count > 0 && (message = _messages.Dequeue()) != null)
                    _repository.Handle(_player, message);
            }
        }

        public Task Send(IMessage message) => _channel.WriteAndFlushAsync(message);
        public Task Close() => _channel.CloseAsync();
    }
}