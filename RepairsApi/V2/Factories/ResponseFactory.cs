using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Domain;
using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Enums;
using Address = RepairsApi.V2.Domain.Address;
using SORPriority = RepairsApi.V2.Domain.SORPriority;

namespace RepairsApi.V2.Factories
{
    public static class ResponseFactory
    {
        public static CautionaryAlertResponseList ToResponse(this AlertList domain)
        {
            return new CautionaryAlertResponseList()
            {
                PropertyReference = domain.PropertyAlerts.PropertyReference,
                LocationAlert = domain.PropertyAlerts.Alerts.Select(alert => alert.ToResponse()).ToList(),
                PersonAlert = domain.PersonAlerts.Alerts.Select(alert => alert.ToResponse()).ToList()
            };
        }

        public static CautionaryAlertViewModel ToResponse(this Alert domain)
        {
            return new CautionaryAlertViewModel
            {
                Type = domain.AlertCode,
                Comments = domain.Description,
                EndDate = domain.EndDate,
                StartDate = domain.StartDate
            };
        }

        public static PropertyViewModel ToResponse(this PropertyModel domain, TenureInformation tenure)
        {
            return new PropertyViewModel
            {
                CanRaiseRepair = (tenure is null) || tenure.CanRaiseRepair, // If there is no tenure then we CAN raise repairs
                PropertyReference = domain.PropertyReference,
                Address = domain.Address.ToResponse(),
                HierarchyType = domain.HierarchyType.ToResponse()
            };
        }

        public static PropertyListItem ToResponseListItem(this PropertyModel domain)
        {
            return new PropertyListItem
            {
                PropertyReference = domain.PropertyReference,
                Address = domain.Address.ToResponse(),
                HierarchyType = domain.HierarchyType.ToResponse()
            };
        }

        public static AddressViewModel ToResponse(this Address domain)
        {
            return new AddressViewModel
            {
                AddressLine = domain.AddressLine,
                PostalCode = domain.PostalCode,
                ShortAddress = domain.ShortAddress,
                StreetSuffix = domain.StreetSuffix
            };
        }

        public static HierarchyTypeViewModel ToResponse(this HierarchyType domain)
        {
            return new HierarchyTypeViewModel
            {
                LevelCode = domain.LevelCode,
                SubTypeCode = domain.SubTypeCode,
                SubTypeDescription = domain.SubTypeDescription
            };
        }

        public static PropertyResponse ToResponse(this PropertyWithAlerts domain)
        {
            return new PropertyResponse
            {
                Property = domain.PropertyModel.ToResponse(domain.Tenure),
                Alerts = new AlertsViewModel
                {
                    LocationAlert = domain.LocationAlerts.Select(alert => alert.ToResponse()).ToList(),
                    PersonAlert = domain.PersonAlerts.Select(alert => alert.ToResponse()).ToList(),
                },
                Tenure = domain.Tenure.ToResponse()
            };
        }


        public static TenureViewModel ToResponse(this TenureInformation domain)
        {
            if (domain == null) return null;

            return new TenureViewModel
            {
                TypeCode = domain.TypeCode,
                TypeDescription = domain.TypeDescription,
                CanRaiseRepair = domain.CanRaiseRepair
            };
        }

        public static List<PropertyListItem> ToResponse(this IEnumerable<PropertyModel> domainList)
        {
            return domainList.Select(domain => domain.ToResponseListItem()).ToList();
        }

        public static WorkOrderListItem ToResponse(this WorkOrder workOrder)
        {
            return new WorkOrderListItem
            {
                Reference = workOrder.Id,
                Description = workOrder.DescriptionOfWork,
                Owner = "", // TODO: populate owner
                Priority = workOrder.WorkPriority.PriorityDescription,
                Property = "", // TODO: populate address
                DateRaised = workOrder.DateRaised,
                LastUpdated = null
            };
        }

        public static WorkPriorityCode ToResponse(this Generated.WorkPriorityCode code)
        {
            return (WorkPriorityCode) code;
        }

        public static ScheduleOfRatesModel ToResponse(this ScheduleOfRates sorCode)
        {
            return new ScheduleOfRatesModel
            {
                CustomCode = sorCode.CustomCode,
                CustomName = sorCode.CustomName,
                SORContractor = new Contractor
                {
                    Reference = sorCode.SORContractorRef
                },
                Priority = new SORPriority
                {
                    Description = sorCode.Priority.Description,
                    PriorityCode = sorCode.Priority.PriorityCode
                }
            };
        }
    }
}
