using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V1.Boundary.Response;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RepairsApi.Tests.V1.E2ETests
{
    [SingleThreaded]
    public class PropertyApitests : IntegrationTests<Startup>
    {
        [Test]
        public void GetSingleProperty()
        {
            var expectedProperty = MockApiGateway.PropertyApiResponse.First();
            PropertyViewModel expectedResponse = expectedProperty.ToDomain().ToResponse();

            ApiGateway client = new ApiGateway(Client);

            var response = client.ExecuteRequest<PropertyResponse>(new Uri($"/api/v2/properties/{expectedProperty.PropRef}", UriKind.Relative)).Result;

            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);

            response.Content.Property.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void GetPropertyWithAlerts()
        {
            var expectedProperty = MockApiGateway.PropertyApiResponse.Last();
            AlertsApiResponse expectedAlerts = DataFakers.StubAlertApiResponse(null, expectedProperty.PropRef).Generate();
            MockApiGateway.AlertsApiResponse.Add(expectedAlerts);
            PropertyViewModel expectedResponse = expectedProperty.ToDomain().ToResponse();
            var expectedAlertResponse = expectedAlerts.ToDomain().ToResponse();

            ApiGateway client = new ApiGateway(Client);

            var response = client.ExecuteRequest<PropertyResponse>(new Uri($"/api/v2/properties/{expectedProperty.PropRef}", UriKind.Relative)).Result;

            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);

            response.Content.Property.Should().BeEquivalentTo(expectedResponse);
            response.Content.CautionaryAlerts.Should().BeEquivalentTo(expectedAlertResponse.Alerts);
        }

        [Test]
        public void Returns404WhenPropertDoesntExist()
        {
            ApiGateway client = new ApiGateway(Client);
            string dummyReference = "dummyReference";

            var response = client.ExecuteRequest<PropertyResponse>(new Uri($"/api/v2/properties/{dummyReference}", UriKind.Relative)).Result;

            response.IsSuccess.Should().BeFalse();
            response.Status.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public void GetAlerts()
        {
            var expectedAlerts = MockApiGateway.AlertsApiResponse.First();
            CautionaryAlertResponseList expectedResponse = expectedAlerts.ToDomain().ToResponse();

            ApiGateway client = new ApiGateway(Client);

            var response = client.ExecuteRequest<PropertyAlertList>(new Uri($"/api/v2/properties/{expectedAlerts.PropertyReference}/alerts", UriKind.Relative)).Result;

            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.PropertyReference.Should().Be(expectedResponse.PropertyReference);
            response.Content.Alerts.Should().BeEquivalentTo(expectedResponse.Alerts);
        }

        [Test]
        public void GetPropertiesBasedOnPostcode()
        {
            const string Postcode = "AA11AA";
            MockApiGateway.AddProperties(5, postcode: Postcode);
            ApiGateway client = new ApiGateway(Client);
            var response = client.ExecuteRequest<List<PropertyViewModel>>(new Uri($"/api/v2/properties/?postcode={Postcode}", UriKind.Relative)).Result;

            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCount(5);
        }

        [Test]
        public void GetPropertiesBasedOnAddress()
        {
            const string Address = "1 road street";
            MockApiGateway.AddProperties(7, address: Address);
            ApiGateway client = new ApiGateway(Client);
            var response = client.ExecuteRequest<List<PropertyViewModel>>(new Uri($"/api/v2/properties/?address={Address}", UriKind.Relative)).Result;

            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCount(7);
        }

        [Test]
        public void GetPropertiesBasedOnAddressQuery()
        {
            const string Address = "2 lane way street";
            MockApiGateway.AddProperties(7, address: Address);
            ApiGateway client = new ApiGateway(Client);
            var response = client.ExecuteRequest<List<PropertyViewModel>>(new Uri($"/api/v2/properties/?q={Address}", UriKind.Relative)).Result;

            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCount(7);
        }

        [Test]
        public void GetPropertiesBasedOnPostcodeQuery()
        {
            const string Postcode = "BB22BB";
            MockApiGateway.AddProperties(5, postcode: Postcode);
            ApiGateway client = new ApiGateway(Client);
            var response = client.ExecuteRequest<List<PropertyViewModel>>(new Uri($"/api/v2/properties/?q={Postcode}", UriKind.Relative)).Result;

            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCount(5);
        }

        // TODO List properties
    }
}
