using System.Collections.Generic;

namespace RepairsApi.V1.Domain.Repair
{
    public class WorkElement
    {
        public IList<RateScheduleItem> RateScheduleItems { get; set; }
    }
}
