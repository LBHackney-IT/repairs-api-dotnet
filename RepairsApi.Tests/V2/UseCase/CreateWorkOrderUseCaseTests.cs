using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Notifications;
using RepairsApi.V2.Services;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trade = RepairsApi.V2.Infrastructure.Trade;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;

namespace RepairsApi.Tests.V2.UseCase
{
    public class CreateWorkOrderUseCaseTests
    {
        private MockRepairsGateway _repairsGatewayMock;
        private Mock<IScheduleOfRatesGateway> _scheduleOfRatesGateway;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private CreateWorkOrderUseCase _classUnderTest;
        private NotificationMock<WorkOrderCreated> _handlerMock;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new MockRepairsGateway();
            _scheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handlerMock = new NotificationMock<WorkOrderCreated>();
            _classUnderTest = new CreateWorkOrderUseCase(
                _repairsGatewayMock.Object,
                _scheduleOfRatesGateway.Object,
                new NullLogger<CreateWorkOrderUseCase>(),
                _currentUserServiceMock.Object,
                _handlerMock
                );
        }

        [Test]
        public async Task Runs()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            var result = await _classUnderTest.Execute(new WorkOrder());

            result.Should().Be(newId);
        }

        [Test]
        public async Task SetsCurrentDate()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            var result = await _classUnderTest.Execute(new WorkOrder());

            VerifyRaiseRepairIsCloseToNow();
        }

        [Test]
        public async Task DoesNotThrowsNotSupportedWhenSingleTradesPosted()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            var workOrder = new WorkOrder
            {
                WorkElements = new List<WorkElement>
                {
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = "trade"}}},
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = "trade"}}}
                }
            };

            var result = await _classUnderTest.Execute(workOrder);

            result.Should().Be(newId);
        }

        [Test]
        public void ThrowsNotSupportedWhenMultipleTradesPosted()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
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

        [Test]
        public async Task SetsRSIToOriginalTrue()
        {
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade> { new Trade { Code = TradeCode.B2 } }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();

            await _classUnderTest.Execute(workOrder);

            _repairsGatewayMock.LastWorkOrder.WorkElements.All(we => we.RateScheduleItem.All(rsi => rsi.Original))
                .Should().BeTrue();
        }


        [Test]
        public async Task HandlersCalled()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            await _classUnderTest.Execute(new WorkOrder());

            _handlerMock.HaveHandlersBeenCalled().Should().BeTrue();
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
