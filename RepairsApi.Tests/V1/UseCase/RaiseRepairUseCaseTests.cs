using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RepairsApi.V1.Domain.Repair;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V1.UseCase
{
    public class RaiseRepairUseCaseTests
    {
        private Mock<IRepairsGateway> _repairsGatewayMock;
        private RaiseRepairUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _classUnderTest = new RaiseRepairUseCase(_repairsGatewayMock.Object, new NullLogger<RaiseRepairUseCase>());
        }

        [Test]
        public async Task Runs()
        {
            int newId = 1;
            _repairsGatewayMock.Setup(m => m.CreateWorkOrder(It.IsAny<WorkOrder>())).ReturnsAsync(newId);
            var result = await _classUnderTest.Execute(new WorkOrder());

            result.Should().Be(newId);
        }
    }
}
