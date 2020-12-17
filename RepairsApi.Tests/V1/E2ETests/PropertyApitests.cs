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
            // Arrange
            var expectedProperty = MockApiGateway.MockPropertyApiResponses.First();
            PropertyViewModel expectedResponse = expectedProperty.ToDomain().ToResponse();

            ApiGateway client = new ApiGateway(Client);

            // Act
            var response = client.ExecuteRequest<PropertyResponse>(new Uri($"/api/v2/properties/{expectedProperty.PropRef}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);

            response.Content.Property.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void GetPropertyWithAlerts()
        {
            // Arrange
            var expectedProperty = MockApiGateway.MockPropertyApiResponses.Last();
            AlertsApiResponse mockAlerts = DataFakers.StubAlertApiResponse(null, expectedProperty.PropRef).Generate();
            MockApiGateway.MockAlertsApiResponses.Add(mockAlerts);

            PropertyViewModel expectedResponse = expectedProperty.ToDomain().ToResponse();
            var expectedAlertResponse = mockAlerts.ToDomain().ToResponse();

            ApiGateway client = new ApiGateway(Client);

            // Act
            var response = client.ExecuteRequest<PropertyResponse>(new Uri($"/api/v2/properties/{expectedProperty.PropRef}", UriKind.Relative)).Result;


            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);

            response.Content.Property.Should().BeEquivalentTo(expectedResponse);
            response.Content.CautionaryAlerts.Should().BeEquivalentTo(expectedAlertResponse.Alerts);
        }

        [Test]
        public void Returns404WhenPropertDoesntExist()
        {
            // Arrange
            ApiGateway client = new ApiGateway(Client);
            string dummyReference = "dummyReference";

            // Act
            var response = client.ExecuteRequest<PropertyResponse>(new Uri($"/api/v2/properties/{dummyReference}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Status.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public void GetAlerts()
        {
            // Arrange
            var expectedAlerts = MockApiGateway.MockAlertsApiResponses.First();
            CautionaryAlertResponseList expectedResponse = expectedAlerts.ToDomain().ToResponse();

            ApiGateway client = new ApiGateway(Client);

            // Act
            var response = client.ExecuteRequest<PropertyAlertList>(new Uri($"/api/v2/properties/{expectedAlerts.PropertyReference}/alerts", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.PropertyReference.Should().Be(expectedResponse.PropertyReference);
            response.Content.Alerts.Should().BeEquivalentTo(expectedResponse.Alerts);
        }

        [Test]
        public void GetPropertiesBasedOnPostcode()
        {
            // Arrange
            const string Postcode = "AA11AA";
            MockApiGateway.AddProperties(5, postcode: Postcode);
            ApiGateway client = new ApiGateway(Client);

            // Act
            var response = client.ExecuteRequest<List<PropertyViewModel>>(new Uri($"/api/v2/properties/?postcode={Postcode}", UriKind.Relative)).Result;

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
            ApiGateway client = new ApiGateway(Client);

            // Act
            var response = client.ExecuteRequest<List<PropertyViewModel>>(new Uri($"/api/v2/properties/?address={Address}", UriKind.Relative)).Result;

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
            ApiGateway client = new ApiGateway(Client);

            // Act
            var response = client.ExecuteRequest<List<PropertyViewModel>>(new Uri($"/api/v2/properties/?q={Address}", UriKind.Relative)).Result;

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
            ApiGateway client = new ApiGateway(Client);

            // Act
            var response = client.ExecuteRequest<List<PropertyViewModel>>(new Uri($"/api/v2/properties/?q={Postcode}", UriKind.Relative)).Result;

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Status.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCount(5);
        }
    }
}
