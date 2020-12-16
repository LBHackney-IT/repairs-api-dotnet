using Bogus;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways.Models;

namespace RepairsApi.Tests.V1
{
    public static class DataFakers
    {

        public static Faker<PropertyAlert> StubAlerts()
        {
            return new Faker<PropertyAlert>()
                .RuleFor(pa => pa.AlertCode, f => f.Random.String())
                .RuleFor(pa => pa.Description, f => f.Random.String())
                .RuleFor(pa => pa.StartDate, f => f.Random.String())
                .RuleFor(pa => pa.EndDate, f => f.Random.String());
        }

        public static Faker<PropertyModel> StubProperties()
        {
            Faker<Address> addresse = StubAddresses();
            Faker<HierarchyType> hierarchyType = StubHierarchies();

            return new Faker<PropertyModel>()
                .RuleFor(pm => pm.Address, f => addresse.Generate())
                .RuleFor(pm => pm.PropertyReference, f => f.Random.Int(0).ToString())
                .RuleFor(pm => pm.HierarchyType, f => hierarchyType.Generate());
        }

        public static Faker<HierarchyType> StubHierarchies()
        {
            return new Faker<HierarchyType>()
                .RuleFor(ht => ht.LevelCode, f => f.Random.String())
                .RuleFor(ht => ht.SubTypeCode, f => f.Random.String())
                .RuleFor(ht => ht.SubTypeDescription, f => f.Random.String());
        }

        public static Faker<Address> StubAddresses()
        {
            return new Faker<Address>()
                .RuleFor(a => a.PostalCode, f => f.Random.String())
                .RuleFor(a => a.ShortAddress, f => f.Random.String())
                .RuleFor(a => a.StreetSuffix, f => f.Random.String())
                .RuleFor(a => a.AddressLine, f => f.Random.String());
        }

        public static PropertyAlertList StubAlertList(string expectedPropertyReference, int alertCount)
        {
            return new PropertyAlertList
            {
                PropertyReference = expectedPropertyReference,
                Alerts = StubAlerts().Generate(alertCount)
            };
        }

        public static Faker<AlertsApiResponse> StubAlertApiResponse(int? alertCount = null)
        {
            Faker<AlertApiAlertViewModel> alertsFake = new Faker<AlertApiAlertViewModel>()
                .RuleFor(pa => pa.AlertCode, f => f.Random.String())
                .RuleFor(pa => pa.Description, f => f.Random.String())
                .RuleFor(pa => pa.StartDate, f => f.Random.String())
                .RuleFor(pa => pa.EndDate, f => f.Random.String());

            return new Faker<AlertsApiResponse>()
                .RuleFor(res => res.PropertyReference, f => f.Random.Int().ToString())
                .RuleFor(res => res.Alerts, f => alertsFake.Generate(alertCount ?? f.Random.Int(0, 20)));
        }

        public static Faker<PropertyApiResponse> StubPropertyApiResponse()
        {
            return new Faker<PropertyApiResponse>()
                .RuleFor(res => res.Address1, f => f.Random.String())
                .RuleFor(res => res.PostCode, f => f.Random.String())
                .RuleFor(res => res.LevelCode, f => f.Random.String())
                .RuleFor(res => res.PropRef, f => f.Random.Int().ToString())
                .RuleFor(res => res.SubtypCode, f => f.PickRandom<string>(ApiModelFactory.HierarchyDescriptions.Keys));
        }
    }
}
