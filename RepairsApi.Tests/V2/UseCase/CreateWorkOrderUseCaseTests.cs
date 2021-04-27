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
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RepairsApi.Tests.V2.UseCase
{
    public class CreateWorkOrderUseCaseTests
    {
        private MockRepairsGateway _repairsGatewayMock;
        private Mock<IAuthorizationService> _authMock;
        private Mock<IScheduleOfRatesGateway> _scheduleOfRatesGateway;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IFeatureManager> _featureManagerMock;
        private CreateWorkOrderUseCase _classUnderTest;
        private NotificationMock<WorkOrderOpened> _handlerMock;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new MockRepairsGateway();
            _authMock = new Mock<IAuthorizationService>();
            _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            _scheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _featureManagerMock = new Mock<IFeatureManager>();
            _featureManagerMock.Setup(fm => fm.IsEnabledAsync(It.IsAny<string>())).ReturnsAsync(true);
            _handlerMock = new NotificationMock<WorkOrderOpened>();
            _classUnderTest = new CreateWorkOrderUseCase(
                _repairsGatewayMock.Object,
                _scheduleOfRatesGateway.Object,
                new NullLogger<CreateWorkOrderUseCase>(),
                _currentUserServiceMock.Object,
                _authMock.Object,
                _featureManagerMock.Object,
                _handlerMock
                );
        }

        [Test]
        public async Task SetsCurrentDate()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            await _classUnderTest.Execute(new WorkOrder());

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

            result.Id.Should().Be(newId);
        }

        [Test]
        public async Task InitialStatusIsOpen()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            await _classUnderTest.Execute(new WorkOrder());

            VerifyPlacedOrder(wo => wo.StatusCode == WorkStatusCode.Open);
        }

        [Test]
        public async Task InitialStatusIsPendingIfOverLimit()
        {
            _authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            await _classUnderTest.Execute(new WorkOrder());

            VerifyPlacedOrder(wo => wo.StatusCode == WorkStatusCode.PendingApproval);
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

        private void VerifyPlacedOrder(Expression<Func<WorkOrder, bool>> predicate)
        {
            _repairsGatewayMock.Verify(m => m.CreateWorkOrder(It.Is<WorkOrder>(predicate)));
        }

        private void VerifyRaiseRepairIsCloseToNow()
        {
            VerifyPlacedOrder(wo => AreDatesClose(DateTime.UtcNow, wo.DateRaised.Value, 60000));
        }

        private static bool AreDatesClose(DateTime d1, DateTime d2, int ms = 60000)
        {
            return Math.Abs((d1 - d2).TotalMilliseconds) < ms;
        }
    }
}
