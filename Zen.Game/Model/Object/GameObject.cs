using Zen.Game.Definition;

namespace Zen.Game.Model.Object
{
    public class GameObject : IEntity
    {
        public GameObject(Position position, int id, int rotation, ObjectType type)
        {
            Position = position;
            Id = id;
            Rotation = rotation;
            Type = type;
        }

        public ObjectType Type { get; }
        public int Rotation { get; }
        public int Id { get; set; }
        public Position Position { get; }
        public ObjectDefinition Definition => ObjectDefinition.ForId(Id);
    }
}