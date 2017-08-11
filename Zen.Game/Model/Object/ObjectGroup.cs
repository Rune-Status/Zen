namespace Zen.Game.Model.Object
{
    public class ObjectGroup
    {
        public static readonly ObjectGroup
            Wall = new ObjectGroup(0),
            WallDecoration = new ObjectGroup(1),
            InteractableObject = new ObjectGroup(2),
            GroundDecoration = new ObjectGroup(3);

        public ObjectGroup(int value) => Value = value;
        public int Value { get; }
    }
}