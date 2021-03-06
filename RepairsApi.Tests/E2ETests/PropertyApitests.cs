using Bogus;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RepairsApi.Tests.E2ETests
{
    [SingleThreaded]
    public class PropertyApitests : MockWebApplicationFactory
    {
        public HttpClient Client => CreateClient();

        [SetUp]
        public void SetUp()
        {
            MockApiGateway.ForcedCode = null;
        }

        [TestCase(8, 7, true)]
        [TestCase(1, 1, false)]
        [TestCase(0, 0, false)]
        [TestCase(10, 0, true)]
        public void GetPropertyWithAlerts(int propertyAlertCount, int personAlertCount, bool canRaiseRepair)
        {
            // Arrange
            var expectedProperty = MockApiGateway.NewProperty();
            string tenantReference = new Faker().Random.String2(10);
            MockApiGateway.AddPropertyAlerts(propertyAlertCount, expectedProperty.PropRef);
            MockApiGateway.AddTenantInformation(tenantReference, expectedProperty.PropRef, canRaiseRepair);
            MockApiGateway.AddPersonAlerts(personAlertCount, tenantReference);
            MockApiGateway.AddResidentContacts();

            PropertyViewModel expectedResponse = expectedProperty.ToDomain().ToResponse(new TenureInformation { CanRaiseRepair = canRaiseRepair });

            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));

            // Act
            var response = client.ExecuteRequest<PropertyResponse>("dummy", new Uri($"/api/v2/properties/{expectedProperty.PropRef}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);

            response.Content.Property.Should().BeEquivalentTo(expectedResponse);
            response.Content.Alerts.LocationAlert.Should().HaveCount(propertyAlertCount);
            response.Content.Alerts.PersonAlert.Should().HaveCount(personAlertCount);
            response.Content.Property.CanRaiseRepair.Should().Be(canRaiseRepair);
        }

        [Test]
        public void Returns404WhenPropertyDoesntExist()
        {
            // Arrange
            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));
            string dummyReference = "dummyReference";

            // Act
            var response = client.ExecuteRequest<PropertyResponse>("dummy", new Uri($"/api/v2/properties/{dummyReference}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Status.Should().Be(HttpStatusCode.NotFound);
        }

        [TestCase(0, 0)]
        [TestCase(1, 2)]
        [TestCase(10, 6)]
        [TestCase(3, 0)]
        [TestCase(28, 50)]
        public void GetAlerts(int expectedPropertyAlertCount, int expectedPersonAlertCount)
        {
            // Arrange
            var expectedProperty = MockApiGateway.NewProperty();
            string tenantReference = new Faker().Random.String2(10);
            MockApiGateway.AddPropertyAlerts(expectedPropertyAlertCount, expectedProperty.PropRef);
            MockApiGateway.AddTenantInformation(tenantReference, expectedProperty.PropRef);
            MockApiGateway.AddPersonAlerts(expectedPersonAlertCount, tenantReference);

            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));

            // Act
            var response = client.ExecuteRequest<CautionaryAlertResponseList>("dummy", new Uri($"/api/v2/properties/{expectedProperty.PropRef}/alerts", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.PropertyReference.Should().Be(expectedProperty.PropRef);
            response.Content.LocationAlert.Should().HaveCount(expectedPropertyAlertCount);
            response.Content.PersonAlert.Should().HaveCount(expectedPersonAlertCount);
        }

        [Test]
        public void GetPropertiesBasedOnPostcode()
        {
            // Arrange
            const string Postcode = "AA11AA";
            MockApiGateway.AddProperties(5, postcode: Postcode);
            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));

            // Act
            var response = client.ExecuteRequest<List<PropertyViewModel>>("dummy", new Uri($"/api/v2/properties/?postcode={Postcode}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCount(5);
        }

        [Test]
        public void GetPropertiesBasedOnAddress()
        {
            // Arrange
            const string Address = "1 road street";
            MockApiGateway.AddProperties(7, address: Address);
            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));

            // Act
            var response = client.ExecuteRequest<List<PropertyViewModel>>("dummy", new Uri($"/api/v2/properties/?address={Address}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCount(7);
        }

        [Test]
        public void GetPropertiesBasedOnAddressQuery()
        {
            // Arrange
            const string Address = "2 lane way street";
            MockApiGateway.AddProperties(7, address: Address);
            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));

            // Act
            var response = client.ExecuteRequest<List<PropertyViewModel>>("dummy", new Uri($"/api/v2/properties/?q={Address}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCount(7);
        }

        [Test]
        public void GetPropertiesBasedOnPostcodeQuery()
        {
            // Arrange
            const string Postcode = "BB22BB";
            MockApiGateway.AddProperties(5, postcode: Postcode);
            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));

            // Act
            var response = client.ExecuteRequest<List<PropertyViewModel>>("dummy", new Uri($"/api/v2/properties/?q={Postcode}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCount(5);
        }

        [Test]
        public void PropertyListForwardsErrorWhenPlatformApiFails()
        {
            // Arrange
            MockApiGateway.ForcedCode = HttpStatusCode.BadGateway;

            const string Postcode = "AA11AA";
            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));

            // Act
            var response = client.ExecuteRequest<List<PropertyViewModel>>("dummy", new Uri($"/api/v2/properties/?postcode={Postcode}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Status.Should().Be(HttpStatusCode.BadGateway);
        }


        [Test]
        public void AlertListForwardsErrorWhenPlatformApiFails()
        {
            // Arrange
            MockApiGateway.ForcedCode = HttpStatusCode.BadGateway;

            var expectedProperty = MockApiGateway.NewProperty();
            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));

            // Act
            var response = client.ExecuteRequest<CautionaryAlertResponseList>("dummy", new Uri($"/api/v2/properties/{expectedProperty.PropRef}/alerts", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Status.Should().Be(HttpStatusCode.BadGateway);
        }

        [Test]
        public void HandlePropertyWithNoSubType()
        {
            // Arrange
            var expectedProperty = MockApiGateway.NewProperty();
            expectedProperty.SubtypCode = string.Empty;

            ApiGateway client = new ApiGateway(new HttpClientFactoryWrapper(Client));

            // Act
            var response = client.ExecuteRequest<PropertyResponse>("dummy", new Uri($"/api/v2/properties/{expectedProperty.PropRef}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
        }

        [TestCase("/api/v2/properties/100/alerts")]
        [TestCase("/api/v2/properties/?postcode=1111")]
        [TestCase("/api/v2/properties/?q=1111")]
        [TestCase("/api/v2/properties/?address=1111")]
        public async Task AuthorisationTests(string test)
        {
            await AuthorisationHelper.VerifyContractorUnauthorised(
                Client,
                GetGroup(TestDataSeeder.Contractor),
                async client =>
                {
                    return await client.GetAsync(new Uri(test, UriKind.Relative));
                });
        }
    }
}
