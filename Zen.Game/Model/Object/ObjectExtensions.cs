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

        public static ObjectGroup ForType(int type)
        {
            if (type >= Groups.Length)
                return default(ObjectGroup);

            var id = Groups[type];
            foreach (var val in Enum.GetValues(typeof(ObjectGroup)))
            {
                if ((int) val == id)
                    return (ObjectGroup) val;
            }

            return default(ObjectGroup);
        }

        public static ObjectGroup GetObjectGroup(this ObjectType type) => ForType((int) type);
    }
}