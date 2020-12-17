using System;
using System.Collections.Generic;
using System.Linq;
using RepairsApi.V1.Boundary.Response;
using RepairsApi.V1.Domain;

namespace RepairsApi.V1.Factories
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
    }
}
