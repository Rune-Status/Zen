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

        protected bool Equals(ObjectGroup other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ObjectGroup) obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}