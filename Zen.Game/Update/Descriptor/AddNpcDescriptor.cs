using Zen.Builder;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Update.Descriptor
{
    public class AddNpcDescriptor : NpcDescriptor
    {
        private readonly int _id;
        private readonly int _type;
        private readonly Direction _direction;
        private readonly Position _position;

        public AddNpcDescriptor(Npc npc) : base(npc)
        {
            _id = npc.Id;
            _type = npc.Type;
            _direction = npc.MostRecentDirection;
            _position = npc.Position;
        }

        public override void EncodeDescriptor(NpcUpdateMessage message, GameFrameBuilder builder,
            GameFrameBuilder blockBuilder)
        {
            var x = _position.X - message.Position.X;
            var y = _position.Y - message.Position.Y;

            builder.PutBits(15, _id);
            builder.PutBit(true);
            builder.PutBits(3, (int) _direction);
            builder.PutBit(BlockUpdateRequired);
            builder.PutBits(5, y);
            builder.PutBits(14, _type);
            builder.PutBits(5, x);
        }
    }
}