using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Zen.Game.Msg;

namespace Zen.Game.Model
{
    public class PlayerSession
    {
        private readonly Queue<Message> _messages = new Queue<Message>();
        private readonly IChannel _channel;

        private readonly Player _player;
        private readonly MessageRepository _repository;

        public PlayerSession(Player player, IChannel channel, MessageRepository repository)
        {
            _player = player;
            _channel = channel;
            _repository = repository;
        }

        public void Enqueue(Message msg)
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
                Message message;
                while (_messages.Count > 0 && (message = _messages.Dequeue()) != null)
                    _repository.Handle(_player, message);
            }
        }

        public Task Send(Message message) => _channel.WriteAndFlushAsync(message);
    }
}