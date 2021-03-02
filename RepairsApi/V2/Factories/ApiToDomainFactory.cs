using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways.Models;
using System.Collections.Generic;
using System.Linq;

namespace RepairsApi.V2.Factories
{
    public static class ApiToDomainFactory
    {
        public static Dictionary<string, string> HierarchyDescriptions => new Dictionary<string, string>
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

        public static HashSet<string> RaisableTenureCodes => new HashSet<string>
        {
            "ASY",
            "COM",
            "DEC",
            "INT",
            "MPA",
            "NON",
            "PVG",
            "SEC",
            "TAF",
            "TGA",
            "TRA"
        };

        public static PropertyAlertList ToDomain(this PropertyAlertsApiResponse apiResponse)
        {
            return new PropertyAlertList
            {
                PropertyReference = apiResponse.PropertyReference,
                Alerts = apiResponse.Alerts.ToDomain()
            };
        }

        public static IEnumerable<Alert> ToDomain(this IEnumerable<AlertApiAlertViewModel> apiResponse)
        {
            return apiResponse.Select(alertResponse => alertResponse.ToDomain());
        }

        public static Alert ToDomain(this AlertApiAlertViewModel apiResponse)
        {
            return new Alert
            {
                AlertCode = apiResponse.AlertCode,
                Description = apiResponse.Description,
                EndDate = apiResponse.EndDate,
                StartDate = apiResponse.StartDate
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
                AddressLine = splitAddress.First(),
                StreetSuffix = splitAddress.Length > 1 ? splitAddress.Last() : string.Empty
            };
        }

        private static HierarchyType ToDomainHierarachy(this PropertyApiResponse apiResponse)
        {
            var subTypeDescription =
                HierarchyDescriptions.ContainsKey(apiResponse.SubtypCode) ?
                HierarchyDescriptions[apiResponse.SubtypCode] :
                "Unknown";

            return new HierarchyType
            {
                LevelCode = apiResponse.LevelCode,
                SubTypeCode = apiResponse.SubtypCode,
                SubTypeDescription = subTypeDescription
            };
        }

        public static PersonAlertList ToDomain(this ListPersonAlertsApiResponse apiResponse)
        {
            return new PersonAlertList
            {
                Alerts = apiResponse.Contacts.First().Alerts.ToDomain()
            };
        }

        public static TenureInformation ToDomain(this ListTenanciesApiResponse apiResponse)
        {
            TenancyApiTenancyInformation tenancyInformation = apiResponse.Tenancies.FirstOrDefault(t => t.Present);

            if (tenancyInformation is null) return null;

            string[] splitTenureType = tenancyInformation.TenureType.Split(": ");
            return new TenureInformation
            {
                TenancyAgreementReference = tenancyInformation.TenancyAgreementReference,
                HouseholdReference = tenancyInformation.HouseholdReference,
                TypeCode = splitTenureType.First(),
                TypeDescription = splitTenureType.Last(),
                CanRaiseRepair = RaisableTenureCodes.Contains(splitTenureType.First())
            };
        }

        public static ResidentContact ToDomain(this ResidentContactInformation apiResponse)
        {
            return new ResidentContact
            {
                FirstName = apiResponse.FirstName,
                LastName = apiResponse.LastName,
                PhoneNumbers = apiResponse.PhoneNumbers.MapList(no => no.PhoneNumber)
            };
        }

        public static List<ResidentContact> ToDomain(this HousingResidentInformationApiResponse apiResponse)
        {
            return apiResponse.Residents.MapList(res => res.ToDomain());
        }
    }
}
