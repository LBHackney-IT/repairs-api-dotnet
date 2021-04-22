using System;
using System.Globalization;

namespace RepairsApi.V2
{
    public static class DateExtensions
    {
        public const string DateFormat = "yyyy-MM-dd";
        public const string TimeFormat = "HH:mm";

        public static string ToDate(this DateTime date)
        {
            return date.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        public static string ToTime(this DateTime date)
        {
            return date.ToString(TimeFormat, CultureInfo.InvariantCulture);
        }
    }
}
