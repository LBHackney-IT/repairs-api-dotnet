using RepairsApi.V1.Domain.Repair;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace RepairsApi.V1.Factories
{
    public static class RequestToDomainFactory
    {
        public static WorkOrder ToDomain(this Generated.RaiseRepair request)
        {
            return new WorkOrder
            {
                DescriptionOfWork = request.DescriptionOfWork,
                SitePropertyUnits = request.SitePropertyUnit.Select(spu => spu.ToDomain()).ToList(),
                WorkClass = request.WorkClass.ToDomain(),
                WorkElements = request.WorkElement.Select(we => we.ToDomain()).ToList(),
                WorkPriority = request.Priority.ToDomain()
            };
        }

        public static SitePropertyUnit ToDomain(this Generated.SitePropertyUnit request)
        {
            return new SitePropertyUnit
            {
                Reference = request.Reference.FirstOrDefault()?.ID
            };
        }

        public static WorkClass ToDomain(this Generated.WorkClass request)
        {
            return new WorkClass
            {
                WorkClassCode = (int)request.WorkClassCode,
            };
        }

        public static WorkElement ToDomain(this Generated.WorkElement request)
        {
            return new WorkElement
            {
                RateScheduleItems = request.RateScheduleItem.Select(rsi => rsi.ToDomain()).ToList(),
            };
        }

        public static RateScheduleItem ToDomain(this Generated.RateScheduleItem request)
        {
            return new RateScheduleItem
            {
                CustomCode = request.CustomCode,
                CustomName = request.CustomName,
                Quantity = request.Quantity.ToDomain()
            };
        }

        public static Quantity ToDomain(this Generated.Quantity request)
        {
            return new Quantity
            {
                Amount = Convert.ToInt32(request.Amount.Single()),
                UnitOfMeasurementCode = GetEnumMemberAttrValue(request.UnitOfMeasurementCode)
            };
        }

        public static WorkPriority ToDomain(this Generated.Priority request)
        {
            return new WorkPriority
            {
                PriorityCode = GetEnumMemberAttrValue(request.PriorityCode),
                RequiredCompletionDateTime = request.RequiredCompletionDateTime
            };
        }

        public static string GetEnumMemberAttrValue<T>(T enumVal)
        {
            var enumType = typeof(T);
            var memInfo = enumType.GetMember(enumVal.ToString());
            var attr = memInfo.FirstOrDefault()?.GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.Value;
            }

            return null;
        }
    }
}
