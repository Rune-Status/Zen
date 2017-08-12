using System.Collections.Generic;
using System.Linq;

namespace Zen.Game.Model.Object
{
    public class ObjectType
    {
        private static readonly Dictionary<int, ObjectType> Types = new Dictionary<int, ObjectType>();

        public static readonly ObjectType
            LengthwiseWall = new ObjectType(0, ObjectGroup.Wall),
            TriangularCorner = new ObjectType(1, ObjectGroup.Wall),
            WallCorner = new ObjectType(2, ObjectGroup.Wall),
            RectangularCorner = new ObjectType(3, ObjectGroup.Wall),
            StraightInside = new ObjectType(4, ObjectGroup.WallDecoration),
            StraightOutside = new ObjectType(5, ObjectGroup.WallDecoration),
            DiagonalOutside = new ObjectType(6, ObjectGroup.WallDecoration),
            DiagonalInside = new ObjectType(7, ObjectGroup.WallDecoration),
            DiagonalInsideWall = new ObjectType(8, ObjectGroup.WallDecoration),
            DiagonalWall = new ObjectType(9, ObjectGroup.InteractableObject),
            Interactable = new ObjectType(10, ObjectGroup.InteractableObject),
            DiagonalInteractable = new ObjectType(11, ObjectGroup.InteractableObject),
            StraightRoof = new ObjectType(12, ObjectGroup.InteractableObject),
            DiagonalRoof = new ObjectType(13, ObjectGroup.InteractableObject),
            DiagonalConnectingRoof = new ObjectType(14, ObjectGroup.InteractableObject),
            StraightConnectingRoof = new ObjectType(15, ObjectGroup.InteractableObject),
            StraightCornerRoof = new ObjectType(16, ObjectGroup.InteractableObject),
            StraightFlatRoof = new ObjectType(17, ObjectGroup.InteractableObject),
            StraightBottomRoof = new ObjectType(18, ObjectGroup.InteractableObject),
            DiagonalBottomRoof = new ObjectType(19, ObjectGroup.InteractableObject),
            StraightBottomConnecting = new ObjectType(20, ObjectGroup.InteractableObject),
            StraightBottomCorner = new ObjectType(21, ObjectGroup.InteractableObject),
            FloorDecoration = new ObjectType(22, ObjectGroup.GroundDecoration);

        public ObjectType(int value, ObjectGroup group)
        {
            Value = value;
            Group = group;
            Types[Value] = this;
        }

        public ObjectGroup Group { get; }
        public int Value { get; }

        protected bool Equals(ObjectType other)
        {
            return Equals(Group, other.Group) && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ObjectType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Group != null ? Group.GetHashCode() : 0) * 397) ^ Value;
            }
        }

        public bool IsWall() => Equals(Group, ObjectGroup.Wall);

        public static ObjectType ForId(int type) => (from kvp in Types where kvp.Key == type select kvp.Value)
            .FirstOrDefault();
    }
}