using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    [TestFixture]
    public class DeleteOperativeUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IOperativeGateway> _operativeGateway;
        private DeleteOperativeUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture.Customize<Operative>(c => c.Without(operative => operative.WorkElement));
            _operativeGateway = new Mock<IOperativeGateway>();
            _classUnderTest = new DeleteOperativeUseCase(_operativeGateway.Object);
        }

        [Test]
        public async Task DeletesOperative()
        {
            // Arrange
            var operativePrn = _fixture.Create<Operative>().PayrollNumber;
            _operativeGateway
                .Setup(gateway => gateway.ArchiveAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var useCaseResult = await _classUnderTest.ExecuteAsync(operativePrn);

            // Assert
            useCaseResult.Should().Be(true);
        }
    }
}
