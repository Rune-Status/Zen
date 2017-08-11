using Zen.Game.Model;
using Zen.Game.Model.Player;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class ObjectOptionOneMessageHandler : MessageHandler<ObjectOptionOneMessage>
    {
        public override void Handle(Player player, ObjectOptionOneMessage message)
        {
            var position = new Position(message.X, message.Y, player.Position.Height);
            var id = message.Id;
            var obj = player.World.GameMap.GetObject(id, position);

            if (obj != null)
            {
                player.SendGameMessage($"Name = {obj.Definition.Name}");
            }
        }
    }
}