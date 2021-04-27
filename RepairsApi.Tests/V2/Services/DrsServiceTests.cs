using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using V2_Generated_DRS;
using RepairsApi.Tests.Helpers.StubGeneration;
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

        [SetUp]
        public void SetUp()
        {
            _drsOptions = Options.Create<DrsOptions>(new DrsOptions
            {
                Login = "login",
                Password = "password"
            });
            _loggerMock = new Mock<ILogger<DrsService>>();
            _drsSoapMock = new MockDrsSoap(_drsOptions);
            _drsMappingMock = new MockDrsMapping();

            _classUnderTest = new DrsService(
                _drsSoapMock.Object,
                _drsOptions,
                _loggerMock.Object,
                _drsMappingMock.Object
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
        public async Task CreateOrder()
        {
            var workOrder = CreateWorkOrderWithContractor(true);
            _drsSoapMock.CreateReturns(responseStatus.success);
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

        private static WorkOrder CreateWorkOrderWithContractor(bool useExternal)
        {
            var expectedContractor = CreateContractor(useExternal);

            var generator = new Generator<WorkOrder>()
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

    }
}
