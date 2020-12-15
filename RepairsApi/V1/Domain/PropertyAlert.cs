namespace RepairsApi.V1.Domain
{
    public class PropertyAlert
    {
        public string AlertCode { get; internal set; }
        public string Description { get; internal set; }
        public string EndDate { get; internal set; } //TODO DateTime
        public string StartData { get; internal set; } //TODO DateTime
    }
}
