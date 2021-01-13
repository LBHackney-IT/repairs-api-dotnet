using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Gateways.Models;
using System;
using System.Linq;
using System.Net;
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
            _apiGatewayMock = new Mock<IApiGateway>();
            _classUnderTest = new AlertsGateway(_apiGatewayMock.Object, new NullLogger<AlertsGateway>());
        }

        [Test]
        public async Task GetsPropertyAlerts()
        {
            // Arrange
            var stubData = BuildResponse(StubPropertyAlertApiResponse(5).Generate());
            _apiGatewayMock.Setup(gw => gw.ExecuteRequest<PropertyAlertsApiResponse>(It.IsAny<string>(), It.IsAny<Uri>())).ReturnsAsync(stubData);

            // Act
            var result = await _classUnderTest.GetLocationAlertsAsync("");

            // Assert
            result.PropertyReference.Should().Be(stubData.Content.PropertyReference);
            result.Alerts.Should().HaveCount(stubData.Content.Alerts.Count);
        }

        [Test]
        public async Task ReturnEmptyPropertyAlertsWhen404()
        {
            // Arrange
            var stubData = BuildResponse<PropertyAlertsApiResponse>(null);
            _apiGatewayMock.Setup(gw => gw.ExecuteRequest<PropertyAlertsApiResponse>(It.IsAny<string>(), It.IsAny<Uri>())).ReturnsAsync(stubData);
            const string expectedPropertyReference = "";

            // Act
            var result = await _classUnderTest.GetLocationAlertsAsync(expectedPropertyReference);

            // Assert
            result.PropertyReference.Should().Be(expectedPropertyReference);
            result.Alerts.Should().BeEmpty();
        }

        [Test]
        public async Task ReturnEmptyListWhenNullTenancyReference()
        {
            // Act
            var result = await _classUnderTest.GetPersonAlertsAsync(null);

            // Assert
            result.Alerts.Should().BeEmpty();
        }

        [TestCase(10)]
        [TestCase(4)]
        [TestCase(0)]
        public async Task ReturnPersonAlertList(int expectedAlertCount)
        {
            // Arrange
            var stubData = BuildResponse(new ListPersonAlertsApiResponse { Contacts = StubPersonAlertApiResponse(expectedAlertCount).Generate(1) });
            _apiGatewayMock.Setup(gw => gw.ExecuteRequest<ListPersonAlertsApiResponse>(It.IsAny<string>(), It.IsAny<Uri>())).ReturnsAsync(stubData);
            const string expectedPropertyReference = "";

            // Act
            var result = await _classUnderTest.GetPersonAlertsAsync(expectedPropertyReference);

            // Assert
            result.Alerts.Should().HaveCount(expectedAlertCount);
        }

        private static ApiResponse<T> BuildResponse<T>(T content) where T : class
        {
            if (content == null)
            {
                return new ApiResponse<T>(false, HttpStatusCode.NotFound, null);
            }

            return new ApiResponse<T>(true, HttpStatusCode.OK, content);
        }
    }
}
