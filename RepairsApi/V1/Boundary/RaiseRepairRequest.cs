using Mapster;
using RepairsApi.V1.Domain.Repair;
using System;
using System.Collections.Generic;

namespace RepairsApi.V1.Boundary
{
    public class RaiseRepairRequest
    {
        public string DescriptionOfWork { get; set; }

        public WorkPriorityViewModel WorkPriority { get; set; }

        public WorkClassViewModel WorkClass { get; set; }

        public IList<WorkElementViewModel> WorkElements { get; set; }

        public IList<SitePropertyUnitViewModel> SitePropertyUnits { get; set; }

        internal WorkOrder ToDomain()
        {
            return this.Adapt<WorkOrder>();
        }
    }

    public class SitePropertyUnitViewModel
    {
        public string Reference { get; set; }
    }

    public class WorkElementViewModel
    {
        public IList<RateScheduleItemViewModel> RateScheduleItems { get; set; }
    }

    public class RateScheduleItemViewModel
    {
        public string CustomCode { get; set; }

        public string CustomName { get; set; }

        public QuantityViewModel Quantity { get; set; }
    }

    public class QuantityViewModel
    {
        public int Amount { get; set; }
        public string UnitOfMeasurementCode { get; set; }
    }

    public class WorkClassViewModel
    {
        public int WorkClassCode { get; set; }
    }

    public class WorkPriorityViewModel
    {
        public string PriorityCode { get; set; }

        public DateTime RequiredCompletionDateTime { get; set; }
    }
}
