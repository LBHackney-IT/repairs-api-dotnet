using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Domain;
using System.Collections.Generic;
using System.Linq;
using RepairsApi.V1.Infrastructure;
using RepairsApi.V2.Enums;
using Address = RepairsApi.V1.Domain.Address;

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

        public static PropertyViewModel ToResponse(this PropertyModel domain)
        {
            return new PropertyViewModel
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
                Property = domain.PropertyModel.ToResponse(),
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
                CanRaiseRepair = domain.CanRaiseRepair,
                TypeCode = domain.TypeCode,
                TypeDescription = domain.TypeDescription
            };
        }

        public static List<PropertyViewModel> ToResponse(this IEnumerable<PropertyModel> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }

        public static WorkOrderListItem ToResponse(this WorkOrder workOrder)
        {
            return new WorkOrderListItem
            {
                Reference = workOrder.Id,
                Description = workOrder.DescriptionOfWork,
                Owner = "", // TODO: populate owner
                Priority = "", // TODO: populate priority
                Property = "", // TODO: populate property
                DateRaised = workOrder.DateReported,
                LastUpdated = null
            };
        }

        public static WorkPriorityCode ToResponse(this Generated.WorkPriorityCode code)
        {
            return (WorkPriorityCode) code;
        }
    }
}
