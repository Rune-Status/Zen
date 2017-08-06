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

        public static long stringToLong(string s)
        {
            long l = 0L;
            for (int i = 0; i < s.Length && i < 12; i++)
            {
                char c = s[i];
                l *= 37L;
                if (c >= 'A' && c <= 'Z')
                    l += (1 + c) - 65;
                else if (c >= 'a' && c <= 'z')
                    l += (1 + c) - 97;
                else if (c >= '0' && c <= '9')
                    l += (27 + c) - 48;
            }
            while (l % 37L == 0L && l != 0L)
                l /= 37L;
            return l;
        }
    }
}