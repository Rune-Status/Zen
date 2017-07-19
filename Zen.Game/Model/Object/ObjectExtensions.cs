using System;
using System.Linq;

namespace Zen.Game.Model.Object
{
    public static class ObjectExtensions
    {
        public static readonly int[] Groups =
        {
            0, 0, 0, 0,
            1, 1, 1, 1, 1,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            3
        };

        public static bool IsWall(this ObjectType type) => ForType((int) type) == ObjectGroup.Wall;

        public static ObjectGroup ForType(int type) => Enum.GetValues(typeof(ObjectGroup))
            .Cast<object>()
            .Where(group => (int) group == Groups[type])
            .Cast<ObjectGroup>().FirstOrDefault();

        public static ObjectGroup GetObjectGroup(this ObjectType type) => ForType((int) type);
    }
}