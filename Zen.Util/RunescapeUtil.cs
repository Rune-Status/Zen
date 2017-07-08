using System.Text;

namespace Zen.Util
{
    public static class RunescapeUtil
    {
        public static string FormatDisplayName(this string str)
        {
            if (str == null) return string.Empty;

            str = str.Replace('_', ' ').ToLower();
            var builder = new StringBuilder();

            var space = true;
            foreach (var @char in str)
            {
                builder.Append(space ? char.ToUpper(@char) : @char);
                space = @char == ' ';
            }
            return builder.ToString();
        }
    }
}