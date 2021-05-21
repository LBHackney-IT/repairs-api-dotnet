using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Request;
using RepairsApi.V2.Filtering;
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
        private Mock<IFilterBuilder<OperativeRequest, Operative>> _filterBuilder;
        private ListOperativesUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture.Customize<Operative>(c => c.Without(operative => operative.WorkElement));
            _operativeGateway = new Mock<IOperativeGateway>();
            _filterBuilder = new Mock<IFilterBuilder<OperativeRequest, Operative>>();
            _classUnderTest = new ListOperativesUseCase(_operativeGateway.Object, _filterBuilder.Object);
        }

        [TestCase(0)]
        [TestCase(5)]
        public async Task ReturnsOperatives(int operativeCount)
        {
            // Arrange
            var operatives = _fixture.CreateMany<Operative>(operativeCount);
            _operativeGateway
                .Setup(gateway => gateway.GetByFilterAsync(It.IsAny<IFilter<Operative>>()))
                .ReturnsAsync(operatives);
            var operativesSearchParams = new OperativeRequest();

            // Act
            var gatewayResult = await _classUnderTest.ExecuteAsync(operativesSearchParams);

            // Assert
            gatewayResult.Should().HaveCount(operativeCount);
        }
    }
}
