using System.Collections.Generic;
using System.Linq;
using RepairsApi.V1.Infrastructure;
using Quantity = RepairsApi.V1.Domain.Repair.Quantity;
using WorkPriorityCode = RepairsApi.V1.Infrastructure.WorkPriorityCode;
using SitePropertyUnit = RepairsApi.V1.Domain.Repair.SitePropertyUnit;
using WorkOrderDB = RepairsApi.V1.Infrastructure.WorkOrder;
using WorkClassDB = RepairsApi.V1.Infrastructure.WorkClass;
using WorkElementDB = RepairsApi.V1.Infrastructure.WorkElement;
using WorkPriorityDB = RepairsApi.V1.Infrastructure.WorkPriority;
using RateScheduleItemDB = RepairsApi.V1.Infrastructure.RateScheduleItem;
using QuantityDB = RepairsApi.V1.Infrastructure.Quantity;
using RateScheduleItem = RepairsApi.V1.Domain.Repair.RateScheduleItem;
using WorkClass = RepairsApi.V1.Domain.Repair.WorkClass;
using WorkElement = RepairsApi.V1.Domain.Repair.WorkElement;
using WorkOrder = RepairsApi.V1.Domain.Repair.WorkOrder;
using WorkPriority = RepairsApi.V1.Domain.Repair.WorkPriority;

namespace RepairsApi.V1.Factories
{
    public static class DBModelFactory
    {
        public static WorkOrderDB ToDb(this WorkOrder domain)
        {
            return new WorkOrderDB
            {
                DescriptionOfWork = domain.DescriptionOfWork,
                WorkPriority = domain.WorkPriority?.ToDb(),
                // TODO: map site
                WorkElements = domain.WorkElements?.ToDb(),
                WorkClass = domain.WorkClass?.ToDb()
            };
        }

        public static WorkPriorityDB ToDb(this WorkPriority domain)
        {
            return new WorkPriorityDB
            {
                PriorityCode = new WorkPriorityCode
                {
                    Name = domain.PriorityCode
                },
                RequiredCompletionDateTime = domain.RequiredCompletionDateTime
            };
        }

        public static List<WorkElementDB> ToDb(this IList<WorkElement> domain)
        {
            return domain.Select(u =>
                new WorkElementDB
                {
                    RateScheduleItem = u.RateScheduleItems.ToDb()
                }
            ).ToList();
        }

        public static List<RateScheduleItemDB> ToDb(this IList<RateScheduleItem> domain)
        {
            return domain.Select(u =>
                new RateScheduleItemDB
                {
                    Quantity = u.Quantity.ToDb(),
                    CustomCode = u.CustomCode,
                    CustomName = u.CustomName
                }
            ).ToList();
        }

        public static WorkClassDB ToDb(this WorkClass domain)
        {
            return new WorkClassDB
            {
                WorkClassCode = domain.WorkClassCode
            };
        }
        public static QuantityDB ToDb(this Quantity domain)
        {
            return new QuantityDB
            {
                Amount = domain.Amount,
                UnitOfMeasurementCode = domain.UnitOfMeasurementCode
            };
        }
    }
}
