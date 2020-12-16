using Bogus;
using Moq;
using NUnit.Framework;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase;
using System.Linq;
using System.Threading.Tasks;

using static RepairsApi.Tests.V1.DataFakers;

namespace RepairsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class ListAlertsUseCaseTests
    {
        private Mock<IAlertsGateway> _alertGatewayMock;
        private ListAlertsUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _alertGatewayMock = new Mock<IAlertsGateway>();
            _classUnderTest = new ListAlertsUseCase(_alertGatewayMock.Object);
        }

        [Test]
        public async Task ReturnsAlerts()
        {
            // Arrange
            const int expectedAlertCount = 5;
            string expectedPropertyReference = new Faker().Random.Number().ToString();
            var expectedAlertList = StubAlertList(expectedPropertyReference, expectedAlertCount);
            _alertGatewayMock.Setup(gm => gm.GetAlertsAsync(It.IsAny<string>())).ReturnsAsync(expectedAlertList);

            // Act
            var result = await _classUnderTest.ExecuteAsync(expectedPropertyReference).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expectedPropertyReference, result.PropertyReference);
            Assert.AreEqual(expectedAlertCount, result.Alerts.Count());
        }
    }
}
