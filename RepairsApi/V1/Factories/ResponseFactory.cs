using System;
using System.Collections.Generic;
using System.Linq;
using RepairsApi.V1.Boundary.Response;
using RepairsApi.V1.Domain;

namespace RepairsApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static CautionaryAlertResponseList ToResponse(this PropertyAlertList domain)
        {
            return new CautionaryAlertResponseList()
            {
                PropertyReference = domain.PropertyReference,
                Alerts = domain.Alerts.Select(alert => alert.ToResponse()).ToList()
            };
        }

        public static CautionaryAlertViewModel ToResponse(this PropertyAlert domain)
        {
            return new CautionaryAlertViewModel
            {
                AlertCode = domain.AlertCode,
                Description = domain.Description,
                EndDate = domain.EndDate,
                StartDate = domain.StartData
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
                CautionaryAlerts = domain.Alerts.Select(alert => alert.ToResponse()).ToList()
            };
        }

        public static List<PropertyViewModel> ToResponse(this IEnumerable<PropertyModel> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
