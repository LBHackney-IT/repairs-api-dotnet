using System;
using FluentAssertions;
using NodaTime;
using NUnit.Framework;
using RepairsApi.V2.Services.DRS;

namespace RepairsApi.Tests.V2.Services
{
    public class DrsHelpersTests
    {
        [TestCase(1, 9, 9)] // when london is in GMT+0 no change
        [TestCase(6, 9, 10)] // when london is in GMT+1 shift an hour
        public void MapsToDrsTime(int month, int hourIn, int expectedHour)
        {
            var timeIn = new DateTime(2020, month, 01, hourIn, 00, 00, DateTimeKind.Utc);
            var expected = new DateTime(2020, month, 01, expectedHour, 00, 00, DateTimeKind.Utc);

            var result = DrsHelpers.ConvertToDrsTimeZone(timeIn);

            result.Should().Be(expected);
        }

        [TestCase(1, 9, 9)] // when london is in GMT+0 no change
        [TestCase(6, 9, 8)] // when london is in GMT+1 shift an hour
        public void MapsFromDrsTime(int month, int hourIn, int expectedHour)
        {
            var timeIn = new DateTime(2020, month, 01, hourIn, 00, 00, DateTimeKind.Utc);
            var expected = new DateTime(2020, month, 01, expectedHour, 00, 00, DateTimeKind.Utc);

            var result = DrsHelpers.ConvertFromDrsTimeZone(timeIn);

            result.Should().Be(expected);
        }
    }
}
