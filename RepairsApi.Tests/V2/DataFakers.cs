using Bogus;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways.Models;
using System;
using System.Linq;

namespace RepairsApi.Tests.V2
{
    public static class DataFakers
    {
        public static Faker<Alert> StubAlerts()
        {
            return new Faker<Alert>()
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
                .RuleFor(pm => pm.HierarchyType, f => hierarchyType.Generate())
                .RuleFor(pm => pm.TmoName, f => f.Random.String());
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

        public static PropertyAlertList StubPropertyAlertList(string expectedPropertyReference, int alertCount)
        {
            return new PropertyAlertList
            {
                PropertyReference = expectedPropertyReference,
                Alerts = StubAlerts().Generate(alertCount)
            };
        }

        public static Faker<PropertyAlertsApiResponse> StubPropertyAlertApiResponse(int? alertCount = null, string propertyReference = null)
        {
            Faker<AlertApiAlertViewModel> alertsFake = new Faker<AlertApiAlertViewModel>()
                .RuleFor(pa => pa.AlertCode, f => f.Random.String2(0, 100))
                .RuleFor(pa => pa.Description, f => f.Random.String2(0, 100))
                .RuleFor(pa => pa.StartDate, f => f.Random.String2(0, 100))
                .RuleFor(pa => pa.EndDate, f => f.Random.String2(0, 100));

            return new Faker<PropertyAlertsApiResponse>()
                .RuleFor(res => res.PropertyReference, f => propertyReference ?? f.Random.Int().ToString())
                .RuleFor(res => res.Alerts, f => alertsFake.Generate(alertCount ?? f.Random.Int(0, 20)));
        }

        public static Faker<PersonAlertsApiResponse> StubPersonAlertApiResponse(int? alertCount = null, string tenancyAgreementReference = null)
        {
            Faker<AlertApiAlertViewModel> alertsFake = new Faker<AlertApiAlertViewModel>()
                .RuleFor(pa => pa.AlertCode, f => f.Random.String2(0, 100))
                .RuleFor(pa => pa.Description, f => f.Random.String2(0, 100))
                .RuleFor(pa => pa.StartDate, f => f.Random.String2(0, 100))
                .RuleFor(pa => pa.EndDate, f => f.Random.String2(0, 100));

            return new Faker<PersonAlertsApiResponse>()
                .RuleFor(res => res.TenancyAgreementReference, f => tenancyAgreementReference ?? f.Random.Int().ToString())
                .RuleFor(res => res.Alerts, f => alertsFake.Generate(alertCount ?? f.Random.Int(0, 20)));
        }

        public static Faker<PropertyApiResponse> StubPropertyApiResponse()
        {
            return new Faker<PropertyApiResponse>()
                .RuleFor(res => res.Address1, f => f.Random.String2(0, 100))
                .RuleFor(res => res.PostCode, f => f.Random.String2(0, 100))
                .RuleFor(res => res.LevelCode, f => f.Random.String2(0, 100))
                .RuleFor(res => res.PropRef, f => f.Random.Int().ToString())
                .RuleFor(res => res.CompAvail, f => $"00{f.Random.Int(0, 9)}")
                .RuleFor(res => res.SubtypCode, f => f.PickRandom<string>(ApiToDomainFactory.HierarchyDescriptions.Keys));
        }

        public static Faker<TenancyApiTenancyInformation> StubTenantApiResponse()
        {
            return new Faker<TenancyApiTenancyInformation>()
                .RuleFor(res => res.TenancyAgreementReference, f => f.Random.Int().ToString())
                .RuleFor(res => res.TenureType, f =>
                {
                    string code = f.Random.Bool() ? f.PickRandom(ApiToDomainFactory.RaisableTenureCodes.AsEnumerable()) : f.Random.String2(3);
                    return $"{code}: {f.Random.Words(10)}";
                });
        }
    }
}
