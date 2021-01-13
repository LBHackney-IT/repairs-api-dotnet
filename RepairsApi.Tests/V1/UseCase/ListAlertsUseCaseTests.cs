using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase;
using System.Threading.Tasks;

using static RepairsApi.Tests.V1.DataFakers;

namespace RepairsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class ListAlertsUseCaseTests
    {
        private Mock<IAlertsGateway> _alertGatewayMock;
        private Mock<ITenancyGateway> _tenancyGatewayMock;
        private ListAlertsUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _alertGatewayMock = new Mock<IAlertsGateway>();
            _tenancyGatewayMock = new Mock<ITenancyGateway>();
            _classUnderTest = new ListAlertsUseCase(_alertGatewayMock.Object, _tenancyGatewayMock.Object);
        }

        [Test]
        public async Task ReturnsAlerts()
        {
            // Arrange
            const int expectedPropertyAlertCount = 5;
            const int expectedPersonAlertCount = 5;
            string expectedPropertyReference = new Faker().Random.Number().ToString();
            var expectedPropertyAlertList = StubPropertyAlertList(expectedPropertyReference, expectedPropertyAlertCount);
            var expectedPersonAlertList = new PersonAlertList { Alerts = StubAlerts().Generate(expectedPersonAlertCount) };
            _alertGatewayMock.Setup(gm => gm.GetLocationAlertsAsync(It.IsAny<string>())).ReturnsAsync(expectedPropertyAlertList);
            _alertGatewayMock.Setup(gm => gm.GetPersonAlertsAsync(It.IsAny<string>())).ReturnsAsync(expectedPersonAlertList);

            // Act
            var result = await _classUnderTest.ExecuteAsync(expectedPropertyReference);

            // Assert
            result.PropertyAlerts.PropertyReference.Should().Be(expectedPropertyReference);
            result.PropertyAlerts.Alerts.Should().HaveCount(expectedPropertyAlertCount);
            result.PersonAlerts.Alerts.Should().HaveCount(expectedPersonAlertCount);
        }
    }
}
