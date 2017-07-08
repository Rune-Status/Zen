using System;

namespace Zen.Util
{
    public static class DateUtil
    {
        private static readonly DateTime TimeEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long CurrentTimeMillis => (long) (DateTime.UtcNow - TimeEpoch).TotalMilliseconds;
    }
}