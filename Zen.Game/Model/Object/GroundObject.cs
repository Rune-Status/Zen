using Zen.Game.Definition;

namespace Zen.Game.Model.Object
{
    public class GroundObject : IEntity
    {
        public GroundObject(Position position, int id, int animationId, int rotation, ObjectType type)
        {
            Position = position;
            Id = id;
            AnimationId = animationId;
            Rotation = rotation;
            Type = type;
        }

        public ObjectDefinition Definition => ObjectDefinition.ForId(Id);
        public ObjectType Type { get; }
        public int Rotation { get; }
        public int AnimationId { get; }
        public int Id { get; }
        public Position Position { get; set; }
        public int Uid { get; set; }
    }
}