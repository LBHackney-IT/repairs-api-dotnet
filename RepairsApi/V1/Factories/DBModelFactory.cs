using RepairsApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepairsApi.V1.Factories
{
    public static class DBModelFactory
    {
        public static WorkOrder ToDb(this Generated.RaiseRepair raiseRepair)
        {
            return new WorkOrder
            {
                DescriptionOfWork = raiseRepair.DescriptionOfWork,
                DateReported = raiseRepair.DateReported,
                EstimatedLaborHours = raiseRepair.EstimatedLaborHours,
                ParkingArrangements = raiseRepair.ParkingArrangements,
                LocationOfRepair = raiseRepair.LocationOfRepair,
                WorkType = raiseRepair.WorkType,
                WorkPriority = raiseRepair.Priority?.ToDb(),
                WorkClass = raiseRepair.WorkClass?.ToDb(),
                Site = raiseRepair.SitePropertyUnit?.ToDb(),
                AccessInformation = raiseRepair.AccessInformation?.ToDb(),
                LocationAlert = raiseRepair.LocationAlert.MapList(la => la.ToDb()),
                PersonAlert = raiseRepair.PersonAlert.MapList(pa => pa.ToDb()),
                WorkElements = raiseRepair.WorkElement.MapList(we => we.ToDb())
            };
        }

        public static Site ToDb(this ICollection<Generated.SitePropertyUnit> raiseRepair)
        {
            return null;
        }

        public static WorkElement ToDb(this Generated.WorkElement raiseRepair)
        {
            return new WorkElement
            {
                ContainsCapitalWork = raiseRepair.ContainsCapitalWork,
                ServiceChargeSubject = raiseRepair.ServiceChargeSubject,
                Trade = raiseRepair.Trade.MapList(t => t.ToDb()),
                RateScheduleItem = raiseRepair.RateScheduleItem.MapList(rsi => rsi.ToDb())
            };
        }

        public static RateScheduleItem ToDb(this Generated.RateScheduleItem raiseRepair)
        {
            return new RateScheduleItem
            {
                CustomCode = raiseRepair.CustomCode,
                CustomName = raiseRepair.CustomName,
                Quantity = raiseRepair.Quantity?.ToDb()
            };
        }

        public static Quantity ToDb(this Generated.Quantity raiseRepair)
        {
            if (raiseRepair.Amount.Count != 1) throw new NotSupportedException("Multiple amounts is not supported");

            return new Quantity
            {
                Amount = raiseRepair.Amount.Single(),
                UnitOfMeasurementCode = raiseRepair.UnitOfMeasurementCode
            };
        }

        public static Trade ToDb(this Generated.Trade raiseRepair)
        {
            return new Trade
            {
                Code = raiseRepair.Code,
                CustomCode = raiseRepair.CustomCode,
                CustomName = raiseRepair.CustomName
            };
        }

        public static AlertRegardingPerson ToDb(this Generated.PersonAlert raiseRepair)
        {
            return new AlertRegardingPerson
            {
                Comments = raiseRepair.Comments,
                Type = raiseRepair.Type
            };
        }

        public static AlertRegardingLocation ToDb(this Generated.LocationAlert raiseRepair)
        {
            return new AlertRegardingLocation
            {
                Comments = raiseRepair.Comments,
                Type = raiseRepair.Type,
                // NOTE: Attachment Not handled
            };
        }

        public static WorkClass ToDb(this Generated.WorkClass raiseRepair)
        {
            return new WorkClass
            {
                WorkClassCode = raiseRepair.WorkClassCode,
                WorkClassDescription = raiseRepair.WorkClassDescription,
                WorkClassSubType = raiseRepair.WorkClassSubType?.ToDb()
            };
        }

        public static WorkClassSubType ToDb(this Generated.WorkClassSubType raiseRepair)
        {
            return new WorkClassSubType
            {
                WorkClassSubTypeDescription = raiseRepair.WorkClassSubTypeDescription,
                WorkClassSubTypeName = string.Join(',', raiseRepair.WorkClassSubType1)
            };
        }

        public static WorkOrderAccessInformation ToDb(this Generated.AccessInformation raiseRepair)
        {
            return new WorkOrderAccessInformation
            {
                Description = raiseRepair.Description,
                Keysafe = raiseRepair.Keysafe?.ToDb()
            };
        }

        public static KeySafe ToDb(this Generated.Keysafe raiseRepair)
        {
            return new KeySafe
            {
                Code = raiseRepair.Code,
                Location = raiseRepair.Location
            };
        }

        public static WorkPriority ToDb(this Generated.Priority raiseRepair)
        {
            return new WorkPriority
            {
                PriorityCode = raiseRepair.PriorityCode,
                RequiredCompletionDateTime = raiseRepair.RequiredCompletionDateTime
            };
        }

        public static List<TResult> MapList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> map)
        {
            if (source is null) return new List<TResult>();

            return source.Select(map).ToList();
        }
    }
}
