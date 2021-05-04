namespace RepairsApi.V2.Domain
{
    public class Alert
    {
        public string AlertCode { get; set; }
        public string Description { get; set; }
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public override string ToString()
        {
            // return $"{AlertCode} - {Description} ({StartDate} - {EndDate})";
            return $"{AlertCode}";
        }
    }
}
