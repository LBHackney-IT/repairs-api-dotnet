namespace RepairsApi.V2.Generated
{
    public partial class Priority
    {
        [Newtonsoft.Json.JsonProperty("PriorityDescription", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string PriorityDescription { get; set; }

        [Newtonsoft.Json.JsonProperty("Comments", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Comments { get; set; }

        [Newtonsoft.Json.JsonProperty("NumberOfDays", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double? NumberOfDays { get; set; }
    }
}
