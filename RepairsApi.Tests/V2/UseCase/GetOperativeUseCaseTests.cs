using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Request;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    [TestFixture]
    public class GetOperativeUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IOperativesGateway> _operativeGateway;
        private GetOperativeUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture.Customize<Operative>(c => c.Without(operative => operative.WorkElement));
            _operativeGateway = new Mock<IOperativesGateway>();
            _classUnderTest = new GetOperativeUseCase(_operativeGateway.Object);
        }

        [Test]
        public async Task ReturnsOperative()
        {
            // Arrange
            var operative = _fixture.Create<Operative>();
            _operativeGateway
                .Setup(gateway => gateway.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(operative);

            // Act
            var useCaseResult = await _classUnderTest.ExecuteAsync(operative.PayrollNumber);

            // Assert
            useCaseResult.Should().BeEquivalentTo(operative.ToResponse());
        }
    }
}
