using System;
using System.Linq;
using System.Threading.Tasks;
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
using RepairsApi.V2.Boundary.Response;
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

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<DrsService>>();
            _drsSoapMock = new MockDrsSoap();
            _drsMappingMock = new Mock<IDrsMapping>();
            _drsOptions = Options.Create<DrsOptions>(new DrsOptions
            {
                Login = "login",
                Password = "password"
            });

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
            VerifyOpenSession(_drsSoapMock.lastOpen).Should().BeTrue();
        }


        [TestCase(responseStatus.failure)]
        [TestCase(responseStatus.error)]
        [TestCase(responseStatus.undefined)]
        public async Task ThrowsApiError_When_SessionFailsToOpen(responseStatus drsResponse)
        {
            string message = "error";
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
            var generator = new Helpers.StubGeneration.Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();
            var workOrder = generator.Generate();

            _drsSoapMock.Setup(x => x.createOrderAsync(It.IsAny<createOrder>()))
                .ReturnsAsync(new createOrderResponse
                {
                    @return = new xmbCreateOrderResponse
                    {
                        status = responseStatus.success
                    }
                });

            await _classUnderTest.CreateOrder(workOrder);

            VerifyOpenSession(_drsSoapMock.lastOpen).Should().BeTrue();
            _drsSoapMock.Verify(x => x.createOrderAsync(It.IsAny<createOrder>()));
        }

        [TestCase(responseStatus.failure)]
        [TestCase(responseStatus.error)]
        [TestCase(responseStatus.undefined)]
        public async Task ThrowsApiError_When_DrsError(responseStatus drsResponse)
        {
            var generator = new Helpers.StubGeneration.Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();
            var workOrder = generator.Generate();
            var errorMsg = "message";
            _drsSoapMock.Setup(x => x.createOrderAsync(It.IsAny<createOrder>()))
                .ReturnsAsync(new createOrderResponse
                {
                    @return = new xmbCreateOrderResponse
                    {
                        status = drsResponse,
                        errorMsg = errorMsg
                    }
                });

            Func<Task> act = async () =>
            {
                await _classUnderTest.CreateOrder(workOrder);
            };

            (await act.Should().ThrowAsync<ApiException>()
                .WithMessage(errorMsg))
                .Which.StatusCode.Should().Be((int) drsResponse);

        }

        private bool VerifyOpenSession(openSession openSession) =>
            openSession.openSession1.login == _drsOptions.Value.Login &&
            openSession.openSession1.password == _drsOptions.Value.Password;


    }

    internal class MockDrsSoap : Mock<SOAP>
    {
        public openSession lastOpen { get; private set; }
        public string sessionId { get; }

        public MockDrsSoap()
        {
            sessionId = Guid.NewGuid().ToString();
            Setup(x => x.openSessionAsync(It.IsAny<openSession>()))
                .Callback<openSession>(o => lastOpen = o)
                .ReturnsAsync(new openSessionResponse
                {
                    @return = new xmbOpenSessionResponse
                    {
                        sessionId = sessionId,
                        status = responseStatus.success
                    }
                });
        }
    }
}
