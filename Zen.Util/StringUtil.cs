using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zen.Util
{
    public static class StringUtil
    {
        public static int Hash(this string key) => key.Aggregate(0, (current, t) => t + ((current << 5) - current));

        public static bool ContainsWord(this string input, string keyword)
        {
            if (input == null) return false;

            var result = Regex.Split(input, @"\s+");
            return result.Any(word => word.ToLower().Equals(keyword.ToLower()));
        }
    }
}