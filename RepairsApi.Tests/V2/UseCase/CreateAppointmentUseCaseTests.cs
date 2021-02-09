using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using System;
using System.Threading.Tasks;

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

            Func<Task> func = async () => await _classUnderTest.Execute(1, 1, DateTime.UtcNow);

            await func.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test]
        public async Task CallGateway()
        {
            _repairGatewayMock.Setup(rgm => rgm.GetWorkOrder(It.IsAny<int>())).ReturnsAsync(new WorkOrder());
            int appointmentId = 500;
            int workOrderId = 500;
            DateTime appointmentDate = new DateTime().AddYears(40).AddMinutes(50);

            await _classUnderTest.Execute(appointmentId, workOrderId, appointmentDate);

            _appointmentsGatewayMock.Verify(agm => agm.Create(appointmentId, workOrderId, appointmentDate));
        }
    }
}
