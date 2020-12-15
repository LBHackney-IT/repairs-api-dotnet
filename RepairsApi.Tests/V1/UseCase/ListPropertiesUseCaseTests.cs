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
    public class ListPropertiesUseCaseTests
    {
        private Mock<IPropertyGateway> _gatewayMock;
        private ListPropertiesUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _gatewayMock = new Mock<IPropertyGateway>();
            _classUnderTest = new ListPropertiesUseCase(_gatewayMock.Object);
        }


        [Test]
        public async Task ReturnsProperties()
        {
            // Arrange
            const int expectedPropertyCount = 5;
            var expectedPropertyList = StubProperties().Generate(expectedPropertyCount);
            _gatewayMock.Setup(gm => gm.GetByQueryAsync(It.IsAny<PropertySearchModel>())).ReturnsAsync(expectedPropertyList);

            // Act
            var result = await _classUnderTest.ExecuteAsync(new PropertySearchModel()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expectedPropertyCount, result.Count());
        }
    }
}
