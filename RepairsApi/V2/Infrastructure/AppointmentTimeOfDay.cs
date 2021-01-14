
using Microsoft.EntityFrameworkCore;
using System;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class AppointmentTimeOfDay
    {
        public string Name { get; set; }
        public DateTime? EarliestArrivalTime { get; set; }
        public DateTime? LatestArrivalTime { get; set; }
        public DateTime? LatestCompletionTime { get; set; }
    }
}

