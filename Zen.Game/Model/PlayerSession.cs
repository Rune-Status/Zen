using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Zen.Game.Msg;

namespace Zen.Game.Model
{
    public class PlayerSession
    {
        private readonly Queue<Message> _messages = new Queue<Message>();

        private readonly Player player;
        private readonly IChannel channel;
        private readonly MessageRepository repository;

        public PlayerSession(Player player, IChannel channel, MessageRepository repository)
        {
            this.player = player;
            this.channel = channel;
            this.repository = repository;
        }

        public void Enqueue(Message msg)
        {
            lock (_messages)
                _messages.Enqueue(msg);
        }

        public void ProcessMessageQueue()
        {
            lock (_messages)
            {
                Message message;
                while (_messages.Count > 0 && (message = _messages.Dequeue()) != null)
                    repository.Handle(player, message);
            }
        }

        public Task Send(Message message) => channel.WriteAndFlushAsync(message);
    }
}