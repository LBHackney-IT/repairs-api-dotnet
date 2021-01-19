using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using System;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    public class RaiseRepairUseCaseTests
    {
        private Mock<IRepairsGateway> _repairsGatewayMock;
        private Mock<IScheduleOfRatesGateway> _sorGatewayMock;
        private RaiseRepairUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _sorGatewayMock = new Mock<IScheduleOfRatesGateway>();
            _classUnderTest = new RaiseRepairUseCase(_repairsGatewayMock.Object, new NullLogger<RaiseRepairUseCase>(), _sorGatewayMock.Object);
        }

        [Test]
        public async Task Runs()
        {
            int newId = 1;
            _repairsGatewayMock.Setup(m => m.CreateWorkOrder(It.IsAny<WorkOrder>())).ReturnsAsync(newId);
            var result = await _classUnderTest.Execute(new WorkOrder());

            result.Should().Be(newId);
        }

        [Test]
        public async Task SetsCurrentDate()
        {
            int newId = 1;
            _repairsGatewayMock.Setup(m => m.CreateWorkOrder(It.IsAny<WorkOrder>())).ReturnsAsync(newId);
            var result = await _classUnderTest.Execute(new WorkOrder());

            VerifyRaiseRepairIsCloseToNow();
        }

        private void VerifyRaiseRepairIsCloseToNow()
        {
            _repairsGatewayMock.Verify(m => m.CreateWorkOrder(It.Is<WorkOrder>(wo => AreDatesClose(DateTime.UtcNow, wo.DateRaised.Value, 60000))));
        }

        private static bool AreDatesClose(DateTime d1, DateTime d2, int ms = 60000)
        {
            return Math.Abs((d1 - d2).TotalMilliseconds) < ms;
        }
    }
}
