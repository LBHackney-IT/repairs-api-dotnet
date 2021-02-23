using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase;
using System.Threading.Tasks;

using static RepairsApi.Tests.V2.DataFakers;

namespace RepairsApi.Tests.V2.UseCase
{
    [TestFixture]
    public class GetPropertyUseCaseTests
    {
        private Mock<IPropertyGateway> _propertyGatewayMock;
        private Mock<IAlertsGateway> _alertGatewayMock;
        private Mock<ITenancyGateway> _tenantGatewayMock;
        private Mock<IResidentContactGateway> _contactGatewayMock;
        private GetPropertyUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _propertyGatewayMock = new Mock<IPropertyGateway>();
            _alertGatewayMock = new Mock<IAlertsGateway>();
            _tenantGatewayMock = new Mock<ITenancyGateway>();
            _contactGatewayMock = new Mock<IResidentContactGateway>();
            _classUnderTest = new GetPropertyUseCase(_propertyGatewayMock.Object, _alertGatewayMock.Object, _tenantGatewayMock.Object, _contactGatewayMock.Object);
        }

        [Test]
        public async Task ReturnsProperty()
        {
            // Arrange
            var expectedProperty = StubProperties().Generate();
            var expectedLocationAlerts = StubPropertyAlertList(expectedProperty.PropertyReference, 5);
            var expectedPersonAlerts = new PersonAlertList() { Alerts = StubAlerts().Generate(5) };
            _propertyGatewayMock.Setup(gm => gm.GetByReferenceAsync(It.IsAny<string>())).ReturnsAsync(expectedProperty);
            _alertGatewayMock.Setup(gm => gm.GetLocationAlertsAsync(It.IsAny<string>())).ReturnsAsync(expectedLocationAlerts);
            _alertGatewayMock.Setup(gm => gm.GetPersonAlertsAsync(It.IsAny<string>())).ReturnsAsync(expectedPersonAlerts);

            // Act
            var result = await _classUnderTest.ExecuteAsync(expectedProperty.PropertyReference);

            // Assert
            result.PropertyModel.Should().Be(expectedProperty);
            result.LocationAlerts.Should().BeEquivalentTo(expectedLocationAlerts.Alerts);
            result.PersonAlerts.Should().BeEquivalentTo(expectedPersonAlerts.Alerts);
        }
    }
}
