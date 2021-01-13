using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase;
using System.Linq;
using System.Threading.Tasks;
using static RepairsApi.Tests.V2.DataFakers;

namespace RepairsApi.Tests.V2.UseCase
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
            int expectedPropertyCount = 5;
            SetupProperties(expectedPropertyCount);
            PropertySearchModel searchModel = new PropertySearchModel
            {
                Query = "DummyQuery"
            };

            // Act
            var result = await _classUnderTest.ExecuteAsync(searchModel);

            // Assert
            result.Should().HaveCount(expectedPropertyCount);
        }

        [Test]
        public async Task ReturnsEmptyListWhenInvalidSearch()
        {
            // Arrange
            SetupProperties(5);

            // Act
            var result = await _classUnderTest.ExecuteAsync(new PropertySearchModel());

            // Assert
            result.Should().BeEmpty();
        }

        private void SetupProperties(int amount)
        {
            var expectedPropertyList = StubProperties().Generate(amount);
            _gatewayMock.Setup(gm => gm.GetByQueryAsync(It.IsAny<PropertySearchModel>())).ReturnsAsync(expectedPropertyList);
        }
    }
}
