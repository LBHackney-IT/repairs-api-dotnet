using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using System;
using System.Threading.Tasks;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.Tests.V2.UseCase
{
    public class CreateAppointmentUseCaseTests
    {
        private Mock<IRepairsGateway> _repairGatewayMock;
        private Mock<IAppointmentsGateway> _appointmentsGatewayMock;
        private CreateAppointmentUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _repairGatewayMock = new Mock<IRepairsGateway>();
            _appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            _classUnderTest = new CreateAppointmentUseCase(_appointmentsGatewayMock.Object, _repairGatewayMock.Object);
        }

        [Test]
        public async Task ThrowsForNoWorkOrder()
        {
            _repairGatewayMock.Setup(rgm => rgm.GetWorkOrder(It.IsAny<int>())).ReturnsAsync((WorkOrder) null);

            Func<Task> func = async () => await _classUnderTest.Execute("1", 1);

            await func.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test]
        public async Task CallGateway()
        {
            _repairGatewayMock.Setup(rgm => rgm.GetWorkOrder(It.IsAny<int>())).ReturnsAsync(new WorkOrder());
            string appointmentId = "500";
            int workOrderId = 500;

            await _classUnderTest.Execute(appointmentId, workOrderId);

            _appointmentsGatewayMock.Verify(agm => agm.Create(appointmentId, workOrderId));
        }
    }
}
