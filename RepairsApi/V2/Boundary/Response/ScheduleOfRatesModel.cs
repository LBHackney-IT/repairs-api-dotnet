using RepairsApi.V2.Domain;

namespace RepairsApi.V2.Boundary.Response
{
    public class ScheduleOfRatesModel
    {
        public string CustomCode { get; set; }
        public string CustomName { get; set; }

        public SORPriority Priority { get; set; }
    }

    public class SorTradeResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
