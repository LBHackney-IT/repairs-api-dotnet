using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase;
using System.Threading.Tasks;

using static RepairsApi.Tests.V1.DataFakers;

namespace RepairsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetPropertyUseCaseTests
    {
        private Mock<IPropertyGateway> _propertyGatewayMock;
        private Mock<IAlertsGateway> _alertGatewayMock;
        private GetPropertyUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _propertyGatewayMock = new Mock<IPropertyGateway>();
            _alertGatewayMock = new Mock<IAlertsGateway>();
            _classUnderTest = new GetPropertyUseCase(_propertyGatewayMock.Object, _alertGatewayMock.Object);
        }

        [Test]
        public async Task ReturnsProperty()
        {
            // Arrange
            var expectedProperty = StubProperties().Generate();
            var expectedAlerts = StubAlertList(expectedProperty.PropertyReference, 5);
            _propertyGatewayMock.Setup(gm => gm.GetByReferenceAsync(It.IsAny<string>())).ReturnsAsync(expectedProperty);
            _alertGatewayMock.Setup(gm => gm.GetAlertsAsync(It.IsAny<string>())).ReturnsAsync(expectedAlerts);

            // Act
            var result = await _classUnderTest.ExecuteAsync(expectedProperty.PropertyReference).ConfigureAwait(false);

            result.PropertyModel.Should().Be(expectedProperty);
            result.Alerts.Should().BeEquivalentTo(expectedAlerts.Alerts);
        }
    }
}
