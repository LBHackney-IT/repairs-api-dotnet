using System;
using System.Globalization;

namespace RepairsApi.V2
{
    public static class DateConstants
    {
        public const string DATEFORMAT = "yyyy-MM-dd";

        public static string ToISO(this DateTime date)
        {
            return date.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}
