using RepairsApi.V1.Domain.Repair;

using WorkOrderDB = RepairsApi.V1.Infrastructure.WorkOrder;

namespace RepairsApi.V1.Factories
{
    public static class DBModelFactory
    {
        public static WorkOrderDB ToDb(this WorkOrder domain)
        {
            return new WorkOrderDB
            {
                DescriptionOfWork = domain.DescriptionOfWork,
                Priority = domain.Priority.ToDb()
                SitePropertyUnits = domain.SitePropertyUnits.ToDb(),
                WorkElements = domain.WorkElements.ToDb(),
                WorkClass = domain.WorkClass.ToDb()
            };
        }
    }
}
