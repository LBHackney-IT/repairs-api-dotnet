using System.Collections.Generic;

namespace RepairsApi.V1.Domain.Repair
{
    public class WorkOrder
    {
        public string DescriptionOfWork { get; set; }

        public Priority Priority { get; set; }

        public WorkClass WorkClass { get; set; }

        public IList<WorkElement> WorkElements { get; set; }

        public IList<SitePropertyUnit> SitePropertyUnits { get; set; }
    }
}
