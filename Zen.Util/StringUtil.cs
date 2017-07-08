using System.Linq;

namespace Zen.Util
{
    public static class StringUtil
    {
        public static int Hash(this string key)
        {
            return key.Aggregate(0, (current, t) => t + ((current << 5) - current));
        }
    }
}