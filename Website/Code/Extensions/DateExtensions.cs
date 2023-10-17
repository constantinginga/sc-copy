using System;

namespace StartupCentral.Extensions
{
    public static class DateExtensions
    {
        public static bool Between(this DateTime value, DateTime oneDate, DateTime otherDate)
        {
            return value >= (oneDate < otherDate ? oneDate : otherDate) && value <= (oneDate > otherDate ? oneDate : otherDate);
        }
    }
}