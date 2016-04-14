using System;

namespace MASFoundation.Internal
{
    public static class DateExtensions
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(this long seconds)
        {
            return Epoch.AddSeconds(seconds);
        }

        public static long ToUnixTime(this DateTime date)
        {
            return Convert.ToInt64((date - Epoch).TotalSeconds);
        }
    }
}
