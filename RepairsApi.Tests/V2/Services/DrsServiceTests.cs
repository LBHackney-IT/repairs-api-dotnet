using System;
using System.Linq;
using System.Linq.Expressions;
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

namespace RepairsApi.Tests.V2.Services
{
    public class DrsServiceTests
    {
        private DrsService _classUnderTest;
        private MockDrsSoap _drsSoapMock;
        private IOptions<DrsOptions> _drsOptions;
        private Mock<ILogger<DrsService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<DrsService>>();
            _drsSoapMock = new MockDrsSoap();
            _drsOptions = Options.Create<DrsOptions>(new DrsOptions
            {
                Login = "login", Password = "password"
            });

            _classUnderTest = new DrsService(_drsSoapMock.Object, _drsOptions, _loggerMock.Object);
        }

        [Test]
        public async Task CreatesSession()
        {
            await _classUnderTest.OpenSession();
            VerifyOpenSession(_drsSoapMock.lastOpen).Should().BeTrue();
        }

        private bool VerifyOpenSession(openSession openSession)
        {
            return
                openSession.openSession1.login == _drsOptions.Value.Login &&
                openSession.openSession1.password == _drsOptions.Value.Password;
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
            _drsSoapMock.Verify(x => x.createOrderAsync(It.Is<createOrder>(o => VerifyCreateOrder(o, workOrder))));
        }

        private bool VerifyCreateOrder(createOrder createOrder, WorkOrder workOrder)
        {
            return
                createOrder.createOrder1.sessionId == _drsSoapMock.sessionId &&
                createOrder.createOrder1.theOrder.primaryOrderNumber == workOrder.Id.ToString();
        }


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
                        sessionId = sessionId
                    }
                });
        }

    }
}
