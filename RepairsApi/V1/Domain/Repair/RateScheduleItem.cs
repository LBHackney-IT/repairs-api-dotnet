namespace RepairsApi.V1.Domain.Repair
{
    public class RateScheduleItem
    {
        public string CustomCode { get; set; }

        public string CustomName { get; set; }

        public Quantity Quantity { get; set; }
    }
}
