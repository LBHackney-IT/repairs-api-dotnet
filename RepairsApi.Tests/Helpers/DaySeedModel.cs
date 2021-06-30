using System;

namespace RepairsApi.Tests.Helpers
{
    internal class DaySeedModel
    {
        public DaySeedModel(DayOfWeek day, int count)
        {
            Day = day;
            Count = count;
        }

        public DayOfWeek Day { get; set; }
        public int Count { get; set; }
    }
}
