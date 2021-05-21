using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    [TestFixture]
    public class ListOperativesUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IOperativeGateway> _operativeGateway;
        private ListOperativesUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture.Customize<Operative>(c => c.Without(operative => operative.WorkElement));
            _operativeGateway = new Mock<IOperativeGateway>();
            _classUnderTest = new ListOperativesUseCase(_operativeGateway.Object);
        }

        [TestCase(0)]
        [TestCase(5)]
        public async Task ReturnsOperatives(int operativeCount)
        {
            // Arrange
            var operatives = _fixture.CreateMany<Operative>(operativeCount);
            _operativeGateway
                .Setup(gateway => gateway.GetByQueryAsync(It.IsAny<RepairsApi.V2.Boundary.Request.OperativeRequest>()))
                .ReturnsAsync(operatives);
            var operativesSearchParams = new RepairsApi.V2.Boundary.Request.OperativeRequest();

            // Act
            var gatewayResult = await _classUnderTest.ExecuteAsync(operativesSearchParams);

            // Assert
            gatewayResult.Should().HaveCount(operativeCount);
        }
    }
}
