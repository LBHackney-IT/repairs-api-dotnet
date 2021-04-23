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
        private Mock<IDrsMapping> _drsMappingMock;
        private Mock<IScheduleOfRatesGateway> _sorGatewayMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<DrsService>>();
            _drsSoapMock = new MockDrsSoap();
            _drsMappingMock = new Mock<IDrsMapping>();
            _sorGatewayMock = new Mock<IScheduleOfRatesGateway>();
            _drsOptions = Options.Create<DrsOptions>(new DrsOptions
            {
                Login = "login",
                Password = "password"
            });

            _classUnderTest = new DrsService(
                _drsSoapMock.Object,
                _drsOptions,
                _loggerMock.Object,
                _drsMappingMock.Object,
                _sorGatewayMock.Object
            );
        }

        [Test]
        public async Task CreatesSession()
        {
            await _classUnderTest.OpenSession();
            VerifyOpenSession(_drsSoapMock.LastOpen).Should().BeTrue();
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

            await _classUnderTest.CreateOrder(workOrder);

            VerifyOpenSession(_drsSoapMock.LastOpen).Should().BeTrue();
            _drsSoapMock.Verify(x => x.createOrderAsync(It.IsAny<createOrder>()));
        }

        [TestCase(responseStatus.failure)]
        [TestCase(responseStatus.error)]
        [TestCase(responseStatus.undefined)]
        public async Task ThrowsApiError_When_DrsError(responseStatus drsResponse)
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

        [TestCase(true)]
        [TestCase(false)]
        public async Task ChecksDBFlagForDrsEnabled(bool useExternal)
        {
            var expectedContractor = WithContractor(useExternal);

            var result = await _classUnderTest.ContractorUsingDrs(expectedContractor.ContractorReference);

            result.Should().Be(expectedContractor.UseExternalScheduleManager);
        }

        [Test]
        public async Task DontOpenSession_When_ContractorNotExternal()
        {
            var workOrder = CreateWorkOrderWithContractor(false);

            await _classUnderTest.CreateOrder(workOrder);

            _drsSoapMock.Verify(x => x.openSessionAsync(It.IsAny<openSession>()), Times.Never);
        }

        private WorkOrder CreateWorkOrderWithContractor(bool useExternal)
        {
            var expectedContractor = WithContractor(useExternal);

            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();
            var workOrder = generator.Generate();

            if (expectedContractor != null)
            {
                workOrder.AssignedToPrimary.ContractorReference = expectedContractor.ContractorReference;
            }

            return workOrder;
        }

        private Contractor WithContractor(bool useExternal)
        {
            Contractor expectedContractor = new Contractor
            {
                ContractorReference = "contractorRef",
                UseExternalScheduleManager = useExternal
            };
            _sorGatewayMock.Setup(x => x.GetContractor(It.IsAny<string>()))
                .ReturnsAsync(expectedContractor);
            return expectedContractor;
        }

        private bool VerifyOpenSession(openSession openSession) =>
            openSession.openSession1.login == _drsOptions.Value.Login &&
            openSession.openSession1.password == _drsOptions.Value.Password;


    }

    internal class MockDrsSoap : Mock<SOAP>
    {
        public openSession LastOpen { get; private set; }
        public string SessionId { get; }

        public MockDrsSoap()
        {
            SessionId = Guid.NewGuid().ToString();
            Setup(x => x.openSessionAsync(It.IsAny<openSession>()))
                .Callback<openSession>(o => LastOpen = o)
                .ReturnsAsync(new openSessionResponse
                {
                    @return = new xmbOpenSessionResponse
                    {
                        sessionId = SessionId,
                        status = responseStatus.success
                    }
                });
        }

        public void CreateReturns(responseStatus status, string errorMsg = null)
        {
            Setup(x => x.createOrderAsync(It.IsAny<createOrder>()))
                .ReturnsAsync(new createOrderResponse
                {
                    @return = new xmbCreateOrderResponse
                    {
                        status = status,
                        errorMsg = errorMsg
                    }
                });
        }
    }
}
