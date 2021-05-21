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
using RepairsApi.V2.Services;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trade = RepairsApi.V2.Infrastructure.Trade;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;
using Party = RepairsApi.V2.Infrastructure.Party;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoFixture;
using Microsoft.Extensions.Options;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Notifications;
using V2_Generated_DRS;

namespace RepairsApi.Tests.V2.UseCase
{
    public class CreateWorkOrderUseCaseTests
    {
        private MockRepairsGateway _repairsGatewayMock;
        private AuthorisationMock _authMock;
        private Mock<IScheduleOfRatesGateway> _scheduleOfRatesGateway;
        private CurrentUserServiceMock _currentUserServiceMock;
        private Mock<IFeatureManager> _featureManagerMock;
        private CreateWorkOrderUseCase _classUnderTest;
        private NotificationMock _notificationMock;
        private DrsOptions _drsOptions;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _repairsGatewayMock = new MockRepairsGateway();
            _authMock = new AuthorisationMock();
            _authMock.SetPolicyResult("RaiseSpendLimit", true);
            _scheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            ContractorUsesExternalScheduler(false);
            _currentUserServiceMock = new CurrentUserServiceMock();
            _featureManagerMock = new Mock<IFeatureManager>();
            _featureManagerMock.Setup(fm => fm.IsEnabledAsync(It.IsAny<string>())).ReturnsAsync(true);
            _notificationMock = new NotificationMock();
            _drsOptions = new DrsOptions
            {
                Login = "login",
                Password = "password",
                APIAddress = new Uri("https://apiAddress.none"),
                ManagementAddress = new Uri("https://managementAddress.none")
            };
            _classUnderTest = new CreateWorkOrderUseCase(
                _repairsGatewayMock.Object,
                _scheduleOfRatesGateway.Object,
                new NullLogger<CreateWorkOrderUseCase>(),
                _currentUserServiceMock.Object,
                _authMock.Object,
                _featureManagerMock.Object,
                _notificationMock,
                Options.Create(_drsOptions)
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
                    new WorkElement
                    {
                        Trade = new List<Trade>
                        {
                            new Trade
                            {
                                CustomCode = "trade"
                            }
                        }
                    },
                    new WorkElement
                    {
                        Trade = new List<Trade>
                        {
                            new Trade
                            {
                                CustomCode = "trade"
                            }
                        }
                    }
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
            _authMock.SetPolicyResult("RaiseSpendLimit", false);
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
                    new WorkElement
                    {
                        Trade = new List<Trade>
                        {
                            new Trade
                            {
                                CustomCode = Guid.NewGuid().ToString()
                            }
                        }
                    },
                    new WorkElement
                    {
                        Trade = new List<Trade>
                        {
                            new Trade
                            {
                                CustomCode = Guid.NewGuid().ToString()
                            }
                        }
                    }
                }
            };

            Assert.ThrowsAsync<NotSupportedException>(async () => await _classUnderTest.Execute(workOrder));
        }

        [Test]
        public async Task SetsRSIToOriginalTrue()
        {
            var generator = new Helpers.StubGeneration.Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade>
                {
                    new Trade
                    {
                        Code = TradeCode.B2
                    }
                }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();

            await _classUnderTest.Execute(workOrder);

            _repairsGatewayMock.LastWorkOrder.WorkElements.All(we => we.RateScheduleItem.All(rsi => rsi.Original))
                .Should().BeTrue();
        }

        [Test]
        public async Task SendOpenedNotification()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);

            await _classUnderTest.Execute(new WorkOrder());

            _notificationMock.HaveHandlersBeenCalled<WorkOrderOpened>().Should().BeTrue();
        }

        [Test]
        public async Task SendHighCostNotificationWhenOverSpendLimit()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            _authMock.SetPolicyResult("RaiseSpendLimit", false);

            await _classUnderTest.Execute(new WorkOrder());

            _notificationMock.HaveHandlersBeenCalled<HighCostWorkOrderCreated>().Should().BeTrue();
        }

        [Test]
        public async Task SetsExternalFlagOnResultWhenContractorHasDrsEnabled()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            ContractorUsesExternalScheduler(true);
            var generator = new Helpers.StubGeneration.Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade>
                {
                    new Trade
                    {
                        Code = TradeCode.B2
                    }
                }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();

            var result = await _classUnderTest.Execute(workOrder);

            result.ExternallyManagedAppointment.Should().BeTrue();
        }

        [Test]
        public async Task AttachesUserInfo()
        {
            var expectedEmail = _fixture.Create<string>();
            var expectedName = _fixture.Create<string>();
            _currentUserServiceMock.SetUser(
                _fixture.Create<string>(),
                expectedEmail,
                expectedName,
                _fixture.Create<string>(),
                _fixture.Create<string>()
            );
            _repairsGatewayMock.ReturnWOId(1);
            var workOrder = new WorkOrder();

            await _classUnderTest.Execute(workOrder);

            workOrder.AgentEmail.Should().Be(expectedEmail);
            workOrder.AgentName.Should().Be(expectedName);
        }

        private void ContractorUsesExternalScheduler(bool external)
        {
            _scheduleOfRatesGateway.Setup(x => x.GetContractor(It.IsAny<string>()))
                .ReturnsAsync(new Contractor
                {
                    UseExternalScheduleManager = external
                });
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
