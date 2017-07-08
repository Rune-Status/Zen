using System;

namespace Zen.Util
{
    public static class Base37Util
    {
        public static string DecodeBase37(this long value)
        {
            var chars = new char[12];
            var pos = 0;

            while (value != 0)
            {
                var remainder = (int) (value % 37);
                value /= 37;

                char c;
                if (remainder >= 1 && remainder <= 26)
                    c = (char) ('a' + remainder - 1);
                else if (remainder >= 27 && remainder <= 36)
                    c = (char) ('0' + remainder - 27);
                else
                    c = '_';

                chars[chars.Length - pos++ - 1] = c;
            }
            return new string(chars, chars.Length - pos, pos);
        }

        public static long EncodeBase37(this string str)
        {
            var len = str.Length;
            if (len > 12)
                throw new ArgumentException("String too long.");

            long value = 0;
            for (var pos = 0; pos < len; pos++)
            {
                var c = str[pos];
                value *= 37;

                if (c >= 'A' && c <= 'Z')
                    value += c - 'A' + 1;
                else if (c >= 'a' && c <= 'z')
                    value += c - 'a' + 1;
                else if (c >= '0' && c <= '9')
                    value += c - '0' + 27;
                else if (c != ' ' && c != '_')
                    throw new ArgumentException("Illegal character in string: " + c + ".");
            }

            while (value != 0 && (value % 37) == 0)
                value /= 37;

            return value;
        }
    }
}