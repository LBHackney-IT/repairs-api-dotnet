using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.XRay.Recorder.Core.Exceptions;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using V2_Generated_DRS;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;

namespace RepairsApi.Tests.V2.Services
{
    public class DrsServiceTests
    {
        private DrsService _classUnderTest;
        private MockDrsSoap _drsSoapMock;
        private IOptions<DrsOptions> _drsOptions;
        private Mock<ILogger<DrsService>> _loggerMock;
        private MockDrsMapping _drsMappingMock;
        private Fixture _fixture;
        private Mock<IOperativesGateway> _operativesGatewayMock;
        private Mock<IAppointmentsGateway> _appointmentsGatewayMock;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _drsOptions = Options.Create<DrsOptions>(new DrsOptions
            {
                Login = "login",
                Password = "password"
            });
            _loggerMock = new Mock<ILogger<DrsService>>();
            _drsSoapMock = new MockDrsSoap(_drsOptions);
            _drsMappingMock = new MockDrsMapping();
            _operativesGatewayMock = new Mock<IOperativesGateway>();
            _appointmentsGatewayMock = new Mock<IAppointmentsGateway>();

            _classUnderTest = new DrsService(
                _drsSoapMock.Object,
                _drsOptions,
                _loggerMock.Object,
                _drsMappingMock.Object,
                _operativesGatewayMock.Object,
                _appointmentsGatewayMock.Object
            );
        }

        [Test]
        public async Task CreatesSession()
        {
            await _classUnderTest.OpenSession();
            _drsSoapMock.VerifyOpenSession();
        }


        [TestCase(responseStatus.failure)]
        [TestCase(responseStatus.error)]
        [TestCase(responseStatus.undefined)]
        public async Task ThrowsApiError_When_SessionFailsToOpen(responseStatus drsResponse)
        {
            const string message = "error";
            _drsSoapMock.Setup(x => x.openSessionAsync(It.IsAny<openSession>()))
                .ReturnsAsync(new openSessionResponse(new xmbOpenSessionResponse
                {
                    status = drsResponse,
                    errorMsg = message
                }));

            Func<Task> act = async () =>
            {
                await _classUnderTest.OpenSession();
            };

            await act.Should().ThrowAsync<ApiException>()
                .WithMessage(message);
        }

        [Test]
        public async Task CreateOrder_Fires_CreateOrderSoapEndpoint()
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            _drsSoapMock.CreateReturns(responseStatus.success);
            _drsSoapMock.UpdateBookingReturns(responseStatus.success);
            _drsMappingMock.SetupMappings(workOrder);

            await _classUnderTest.CreateOrder(workOrder);

            _drsSoapMock.VerifyOpenSession();
            _drsSoapMock.Verify(x => x.createOrderAsync(It.Is<createOrder>(c => c.createOrder1.id == workOrder.Id)));
        }

        [TestCase(responseStatus.failure)]
        [TestCase(responseStatus.error)]
        [TestCase(responseStatus.undefined)]
        public async Task ThrowsApiError_When_CreateDrsError(responseStatus drsResponse)
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            const string errorMsg = "message";
            _drsSoapMock.CreateReturns(drsResponse, errorMsg);

            Func<Task> act = async () =>
            {
                await _classUnderTest.CreateOrder(workOrder);
            };

            (await act.Should().ThrowAsync<ApiException>()
                    .WithMessage(errorMsg))
                .Which.StatusCode.Should().Be((int) drsResponse);

        }

        [Test]
        public async Task CancelsOrder()
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            _drsSoapMock.DeleteReturns(responseStatus.success);
            _drsMappingMock.SetupMappings(workOrder);

            await _classUnderTest.CancelOrder(workOrder);

            _drsSoapMock.VerifyOpenSession();
            _drsSoapMock.Verify(x => x.deleteOrderAsync(It.Is<deleteOrder>(d => d.deleteOrder1.id == workOrder.Id)));
        }

        [TestCase(responseStatus.failure)]
        [TestCase(responseStatus.error)]
        [TestCase(responseStatus.undefined)]
        public async Task ThrowsApiError_When_DeleteDrsError(responseStatus drsResponse)
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            const string errorMsg = "message";
            _drsSoapMock.DeleteReturns(drsResponse, errorMsg);

            Func<Task> act = async () =>
            {
                await _classUnderTest.CancelOrder(workOrder);
            };

            (await act.Should().ThrowAsync<ApiException>()
                    .WithMessage(errorMsg))
                .Which.StatusCode.Should().Be((int) drsResponse);

        }

        [Test]
        public async Task CompletesOrder()
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            var drsOrder = _fixture.Create<order>();
            drsOrder.status = orderStatus.PLANNED;

            _drsSoapMock.UpdateBookingReturns(responseStatus.success);
            _drsSoapMock.SelectOrderReturns(drsOrder);
            _drsMappingMock.SetupMappings(workOrder);

            await _classUnderTest.CompleteOrder(workOrder);

            _drsSoapMock.VerifyOpenSession();
            _drsSoapMock.Verify(x => x.updateBookingAsync(It.Is<updateBooking>(d => d.updateBooking1.theBooking.theOrder.orderId == workOrder.Id)));
            _drsSoapMock.Verify(x => x.selectOrderAsync(It.Is<selectOrder>(s => s.selectOrder1.primaryOrderNumber.Contains(workOrder.Id.ToString()))));
            _drsMappingMock.Verify(x => x.BuildCompleteOrderUpdateBookingRequest(It.IsAny<string>(), It.IsAny<WorkOrder>(), drsOrder));
        }

        [TestCase(responseStatus.failure)]
        [TestCase(responseStatus.error)]
        [TestCase(responseStatus.undefined)]
        public async Task ThrowsApiError_When_UpdateBookingDrsError(responseStatus drsResponse)
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            var drsOrder = _fixture.Create<order>();
            drsOrder.status = orderStatus.PLANNED;
            const string errorMsg = "message";
            _drsSoapMock.UpdateBookingReturns(drsResponse, errorMsg);
            _drsSoapMock.SelectOrderReturns(drsOrder);

            Func<Task> act = () => _classUnderTest.CompleteOrder(workOrder);

            (await act.Should().ThrowAsync<ApiException>().WithMessage(errorMsg))
                .Which.StatusCode.Should().Be((int) drsResponse);
        }

        [TestCase(responseStatus.failure)]
        [TestCase(responseStatus.error)]
        [TestCase(responseStatus.undefined)]
        public async Task ThrowsApiError_When_SelectOrderDrsError(responseStatus drsResponse)
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            var drsOrder = _fixture.Create<order>();
            drsOrder.status = orderStatus.PLANNED;
            const string errorMsg = "message";
            _drsSoapMock.SelectOrderReturns(null, drsResponse, errorMsg);

            Func<Task> act = () => _classUnderTest.CompleteOrder(workOrder);

            (await act.Should().ThrowAsync<ApiException>().WithMessage(errorMsg))
                .Which.StatusCode.Should().Be((int) drsResponse);
        }

        [Test]
        public async Task CancelsWhenOrderNotPlanned()
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            var drsOrder = _fixture.Create<order>();
            foreach (var booking in drsOrder.theBookings)
            {
                booking.theResources = null;
            }
            _drsSoapMock.DeleteReturns(responseStatus.success);
            _drsSoapMock.SelectOrderReturns(drsOrder);
            _drsMappingMock.SetupMappings(workOrder);

            await _classUnderTest.CompleteOrder(workOrder);

            _drsSoapMock.Verify(x => x.deleteOrderAsync(It.Is<deleteOrder>(d => d.deleteOrder1.id == workOrder.Id)));
        }

        [Test]
        public async Task SelectsOrder()
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            var drsOrder = _fixture.Create<order>();

            _drsSoapMock.SelectOrderReturns(drsOrder);

            var result = await _classUnderTest.SelectOrder(workOrder.Id);

            _drsSoapMock.VerifyOpenSession();
            _drsSoapMock.Verify(x => x.selectOrderAsync(It.Is<selectOrder>(s => s.selectOrder1.primaryOrderNumber.Contains(workOrder.Id.ToString()))));
            result.Should().BeEquivalentTo(drsOrder);
        }

        [Test]
        public async Task UpdatesAssignedOperativeAndBookingWindow()
        {
            const int workOrderId = 1234;
            const int operativeId = 5678;
            const string operativePayrollId = "Z123";
            var drsOrder = CreateDrsOrder(operativePayrollId);
            _drsSoapMock.SelectOrderReturns(drsOrder);
            _operativesGatewayMock.Setup(x => x.GetAsync(operativePayrollId))
                .ReturnsAsync(new Operative
                {
                    Id = operativeId
                });

            await _classUnderTest.UpdateWorkOrderDetails(workOrderId);

            _operativesGatewayMock.Verify(x => x.AssignOperatives(workOrderId, operativeId));

            var booking = drsOrder.theBookings.Single();
            _appointmentsGatewayMock.Verify((x => x.SetTimedBooking(workOrderId, booking.planningWindowStart, booking.planningWindowEnd)));
        }

        [Test]
        public async Task DoesNotAssignWhenOrderHasNoResource()
        {
            const int workOrderId = 1234;
            const int operativeId = 5678;
            const string operativePayrollId = "Z123";
            var drsOrder = _fixture.Create<order>();
            drsOrder.primaryOrderNumber = workOrderId.ToString();
            drsOrder.theBookings = new[]
            {
                new booking
                {
                }
            };
            _drsSoapMock.SelectOrderReturns(drsOrder);

            _operativesGatewayMock.Setup(x => x.GetAsync(operativePayrollId))
                .ReturnsAsync(new Operative
                {
                    Id = operativeId
                });

            await _classUnderTest.UpdateWorkOrderDetails(int.Parse(drsOrder.primaryOrderNumber));

            _operativesGatewayMock.Verify(x => x.AssignOperatives(It.IsAny<int>(), It.IsAny<int[]>()), Times.Never);
        }

        private static WorkOrder CreateWorkOrderWithContractor(bool useExternal)
        {
            var expectedContractor = CreateContractor(useExternal);

            var generator = new Helpers.StubGeneration.Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();
            var workOrder = generator.Generate();

            if (expectedContractor != null)
            {
                workOrder.AssignedToPrimary.ContractorReference = expectedContractor.ContractorReference;
            }

            return workOrder;
        }

        private static Contractor CreateContractor(bool useExternal)
        {
            Contractor expectedContractor = new Contractor
            {
                ContractorReference = "contractorRef",
                UseExternalScheduleManager = useExternal
            };
            return expectedContractor;
        }

        private static order CreateDrsOrder(params string[] payrollIds)
        {
            var now = DateTime.UtcNow;
            return new order
            {
                theBookings = new[]
                {
                    new booking
                    {
                        planningWindowStart = now,
                        planningWindowEnd = now.AddHours(5),
                        theResources = payrollIds.Select(ori =>
                            new resource
                            {
                                externalResourceCode = ori
                            }).ToArray()
                    }
                }
            };
        }
    }
}
