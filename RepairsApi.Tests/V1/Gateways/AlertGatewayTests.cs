using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

using static RepairsApi.Tests.V1.DataFakers;

namespace RepairsApi.Tests.V1.Gateways
{
    public class AlertGatewayTests
    {
        private static Uri AlertUri => new Uri("http://alerttest/");
        private static Uri PropertyUri => new Uri("http://propertytest/");
        private Mock<IApiGateway> _apiGatewayMock;
        private AlertsGateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            var gatewayOptions = new GatewayOptions
            {
                AlertsApi = AlertUri,
                PropertiesAPI = PropertyUri,
            };

            _apiGatewayMock = new Mock<IApiGateway>();
            _classUnderTest = new AlertsGateway(Options.Create(gatewayOptions), _apiGatewayMock.Object);
        }

        [Test]
        public async Task SendsRequest()
        {
            // Arrange
            AlertsApiResponse stubData = StubAlertApiResponse(5);
            _apiGatewayMock.Setup(gw => gw.ExecuteRequest<AlertsApiResponse>(It.IsAny<Uri>())).ReturnsAsync(stubData);

            // Act
            var result = await _classUnderTest.GetAlertsAsync("").ConfigureAwait(false);

            // Assert
            result.PropertyReference.Should().Be(stubData.PropertyReference);
            result.Alerts.Should().HaveCount(stubData.Alerts.Count);
        }
    }
}
