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
            if (GameWorld.Instance.TraversalMap.ShouldModifyPlane(position.X, position.Y))
                position = position.Translate(0, 0, 1);

            var groundObject = GameWorld.Instance.GroundObjects.Get(message.Id, position);
            if (groundObject == null) return;

            player.SendGameMessage($"Object = {groundObject.Definition.Name}");
        }
    }
}