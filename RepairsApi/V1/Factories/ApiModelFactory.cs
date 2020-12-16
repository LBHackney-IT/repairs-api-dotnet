using RepairsApi.V1.Domain;
using RepairsApi.V1.Gateways.Models;
using System.Collections.Generic;
using System.Linq;

namespace RepairsApi.V1.Factories
{
    public static class ApiModelFactory
    {
        private static Dictionary<string, string> _hierarchyDescriptions = new Dictionary<string, string>
        {
            {"ADM", "Administrative Building" },
            {"BLK", "Block" },
            {"BLR", "Boiler House" },
            {"BOO", "Booster Pump" },
            {"CHP", "Combined Heat and Power Unit." },
            {"CLF", "Cleaners Facilities" },
            {"CMH", "Community Hall" },
            {"CON", "Concierge" },
            {"DWE", "Dwelling" },
            {"EST", "Estate" },
            {"HIR", "High Rise Block (6 storeys or more)" },
            {"LFT", "Lift" },
            {"LND", "Lettable Non-Dwelling" },
            {"LOR", "Low Rise Block (2 storeys or less)" },
            {"MER", "Medium Rise Block (3-5 storeys)" },
            {"PLA", "Playground" },
            {"TER", "Terraced Block" },
            {"TRA", "Traveller Site" },
            {"WLK", "Walk-Up Block" }
        };

        public static PropertyAlertList ToDomain(this AlertsApiResponse apiResponse)
        {
            return new PropertyAlertList
            {
                PropertyReference = apiResponse.PropertyReference,
                Alerts = apiResponse.Alerts.Select(alertResponse => alertResponse.ToDomain())
            };
        }

        public static PropertyAlert ToDomain(this AlertApiAlertViewModel apiResponse)
        {
            return new PropertyAlert
            {
                AlertCode = apiResponse.AlertCode,
                Description = apiResponse.Description,
                EndDate = apiResponse.EndDate,
                StartData = apiResponse.StartDate
            };
        }

        public static List<PropertyModel> ToDomain(this List<PropertyApiResponse> apiResponse)
        {
            return apiResponse.Select(property => property.ToDomain()).ToList();
        }

        public static PropertyModel ToDomain(this PropertyApiResponse apiResponse)
        {
            return new PropertyModel
            {
                PropertyReference = apiResponse.PropRef,
                Address = apiResponse.ToDomainAddress(),
                HierarchyType = apiResponse.ToDomainHierarachy()
            };
        }

        private static Address ToDomainAddress(this PropertyApiResponse apiResponse)
        {
            string[] splitAddress = apiResponse.Address1.Split("  ");
            return new Address
            {
                ShortAddress = apiResponse.Address1,
                PostalCode = apiResponse.PostCode,
                AddressLine = apiResponse.Address1.Split(" ").First(),
                StreetSuffix = splitAddress.Length > 1 ? splitAddress.Last() : string.Empty
            };
        }

        private static HierarchyType ToDomainHierarachy(this PropertyApiResponse apiResponse)
        {
            return new HierarchyType
            {
                LevelCode = apiResponse.LevelCode,
                SubTypeCode = apiResponse.SubtypCode,
                SubTypeDescription = _hierarchyDescriptions[apiResponse.SubtypCode]
            };
        }
    }
}
