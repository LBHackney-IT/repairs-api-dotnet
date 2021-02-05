using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.MiddleWare;
using RepairsApi.V2.Services;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    public class RaiseRepairUseCaseTests
    {
        private Mock<IRepairsGateway> _repairsGatewayMock;
        private Mock<IScheduleOfRatesGateway> _scheduleOfRatesGateway;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private CreateWorkOrderUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _scheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _classUnderTest = new CreateWorkOrderUseCase(
                _repairsGatewayMock.Object,
                _scheduleOfRatesGateway.Object,
                new NullLogger<CreateWorkOrderUseCase>(),
                _currentUserServiceMock.Object
                );
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

        [Test]
        public async Task DoesNotThrowsNotSupportedWhenSingleTradesPosted()
        {
            int newId = 1;
            _repairsGatewayMock.Setup(m => m.CreateWorkOrder(It.IsAny<WorkOrder>())).ReturnsAsync(newId);
            var workOrder = new WorkOrder
            {
                WorkElements = new List<WorkElement>
                {
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = "trade"}}},
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = "trade"}}}
                }
            };

            var result = await _classUnderTest.Execute(new WorkOrder());

            result.Should().Be(newId);
        }

        [Test]
        public void ThrowsNotSupportedWhenMultipleTradesPosted()
        {
            int newId = 1;
            _repairsGatewayMock.Setup(m => m.CreateWorkOrder(It.IsAny<WorkOrder>())).ReturnsAsync(newId);
            var workOrder = new WorkOrder
            {
                WorkElements = new List<WorkElement>
                {
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = Guid.NewGuid().ToString()}}},
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = Guid.NewGuid().ToString()}}}
                }
            };

            Assert.ThrowsAsync<NotSupportedException>(async () => await _classUnderTest.Execute(workOrder));
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
