namespace RepairsApi.V2.Boundary.Response
{
    public class LegacyScheduleOfRatesModel
    {
        public string CustomCode { get; set; }
        public string CustomName { get; set; }

        public LegacySORPriority Priority { get; set; }

        public LegacyContractor SORContractor { get; set; }
    }

    public class LegacyContractor
    {
        public string Reference { get; set; }
    }

    public class LegacySORPriority
    {
        public int PriorityCode { get; set; }

        public string Description { get; set; }
    }
}
