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
        private Mock<IPropertyGateway> _gatewayMock;
        private GetPropertyUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _gatewayMock = new Mock<IPropertyGateway>();
            _classUnderTest = new GetPropertyUseCase(_gatewayMock.Object);
        }

        [Test]
        public async Task ReturnsProperty()
        {
            // Arrange
            var expectedProperty = StubProperties().Generate();
            var expectedAlerts = StubAlertList(expectedProperty.PropertyReference, 5);
            _gatewayMock.Setup(gm => gm.GetByReferenceAsync(It.IsAny<string>())).ReturnsAsync(expectedProperty);
            _gatewayMock.Setup(gm => gm.GetAlertsAsync(It.IsAny<string>())).ReturnsAsync(expectedAlerts);

            // Act
            var result = await _classUnderTest.ExecuteAsync(expectedProperty.PropertyReference).ConfigureAwait(false);

            Assert.AreEqual(expectedProperty, result.PropertyModel);
            Assert.AreEqual(expectedAlerts.Alerts, result.Alerts);
        }
    }
}
