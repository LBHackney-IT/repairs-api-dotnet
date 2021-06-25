using System;
using NodaTime;

namespace RepairsApi.V2.Services.DRS
{
    public static class DrsHelpers
    {
        public static DateTime ConvertToDrsTimeZone(DateTime dateTime)
        {
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            var utcDateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
            var local = Instant.FromDateTimeUtc(utcDateTime).InUtc();
            return local.WithZone(london).ToDateTimeUnspecified();
        }

        public static DateTime ConvertFromDrsTimeZone(DateTime dateTime)
        {
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            var utcDateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
            return dateTime - london.GetUtcOffset(Instant.FromDateTimeUtc(utcDateTime)).ToTimeSpan();
        }
    }
}
