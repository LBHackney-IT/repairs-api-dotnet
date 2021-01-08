using System.ComponentModel.DataAnnotations;
using LocationAlertTypeCode = RepairsApi.V1.Generated.LocationAlertTypeCode;

namespace RepairsApi.V1.Infrastructure
{
    public class AlertRegardingLocation
    {
        [Key] public int Id { get; set; }
        public string Comments { get; set; }
        // TODO: Missing out attachments as this raises many other questions and is currently out of scope
        public LocationAlertTypeCode Type { get; set; }
    }
}
