using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V1.Boundary.Response;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V1.E2ETests
{
    public class PropertyApitests : IntegrationTests<Startup>
    {
        [Test]
        public async Task GetSingleProperty()
        {
            var expectedProperty = MockApiGateway.PropertyApiResponse.Value.First();
            PropertyViewModel expectedResponse = expectedProperty.ToDomain().ToResponse();

            ApiGateway client = new ApiGateway(Client);

            var response = await client.ExecuteRequest<PropertyResponse>(new Uri($"/api/v1/properties/{expectedProperty.PropRef}", UriKind.Relative));

            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);

            response.Content.Property.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public async Task GetAlerts()
        {
            var expectedAlerts = MockApiGateway.AlertsApiResponse.Value.First();
            CautionaryAlertResponseList expectedResponse = expectedAlerts.ToDomain().ToResponse();

            ApiGateway client = new ApiGateway(Client);

            var response = await client.ExecuteRequest<PropertyAlertList>(new Uri($"/api/v1/properties/{expectedAlerts.PropertyReference}/alerts", UriKind.Relative));

            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.PropertyReference.Should().Be(expectedResponse.PropertyReference);
            response.Content.Alerts.Should().BeEquivalentTo(expectedResponse.Alerts);
        }

        // List properties

        // Check alerts on property by ref
    }
}
