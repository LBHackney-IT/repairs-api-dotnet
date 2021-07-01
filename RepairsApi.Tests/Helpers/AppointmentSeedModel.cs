using System;

namespace RepairsApi.Tests.Helpers
{
    internal class AppointmentSeedModel
    {
        public AppointmentSeedModel(string description, DateTime startTime, DateTime endTime)
        {
            Description = description;
            StartTime = startTime;
            EndTime = endTime;
        }

        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
