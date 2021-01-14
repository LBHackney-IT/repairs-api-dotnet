
using System;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class AppointmentTimeOfDay
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EarliestArrivalTime { get; set; }
        public DateTime LatestArrivalTime { get; set; }
        public DateTime LatestCompletionTime { get; set; }
    }


}

