using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Descriptor
{
    public class AddPlayerDescriptor : PlayerDescriptor
    {
        private readonly Direction _direction;
        private readonly int _id;
        private readonly Position _position;

        public AddPlayerDescriptor(Player player, int[] tickets) : base(player, tickets)
        {
            _id = player.Id;
            _direction = player.MostRecentDirection;
            _position = player.Position;
        }

        public override void EncodeDescriptor(PlayerUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            var x = _position.X - message.Position.X;
            var y = _position.Y - message.Position.Y;

            builder.PutBits(11, _id)
                .PutBits(1, BlockUpdateRequired ? 1 : 0)
                .PutBits(5, x)
                .PutBits(3, (int) _direction)
                .PutBits(1, 1)
                .PutBits(5, y);
        }
    }
}