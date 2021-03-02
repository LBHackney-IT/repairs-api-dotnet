using System;
using System.Globalization;

namespace RepairsApi.V2
{
    public static class DateExtensions
    {
        public const string DATEFORMAT = "yyyy-MM-dd";
        public const string TIMEFORMAT = "HH:mm";

        public static string ToDate(this DateTime date)
        {
            return date.ToString(DATEFORMAT, CultureInfo.InvariantCulture);
        }

        public static string ToTime(this DateTime date)
        {
            return date.ToString(TIMEFORMAT, CultureInfo.InvariantCulture);
        }
    }
}
