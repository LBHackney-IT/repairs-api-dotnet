using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    public class ListAppointmentsUseCaseTests
    {
        private Mock<IRepairsGateway> _repairGatewayMock;
        private Mock<IAppointmentsGateway> _appointmentsGatewayMock;
        private ListAppointmentsUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _repairGatewayMock = new Mock<IRepairsGateway>();
            _appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            _classUnderTest = new ListAppointmentsUseCase(_repairGatewayMock.Object, _appointmentsGatewayMock.Object);
        }

        [Test]
        public async Task ThrowsIfNoWorkOrder()
        {
            _repairGatewayMock.Setup(rgm => rgm.GetWorkOrder(It.IsAny<int>())).ReturnsAsync((WorkOrder) null);
            int workOrder = 0;
            Func<Task> testFn = async () => await _classUnderTest.Execute(workOrder, DateTime.UtcNow, DateTime.UtcNow);

            await testFn.Should().ThrowAsync<ResourceNotFoundException>();
        }

        //[Test]
        //public async Task ReturnsList()
        //{
        //    DateTime toDate = DateTime.UtcNow;
        //    DateTime fromDate = DateTime.UtcNow;
        //    int workOrder = 0;
        //    _repairGatewayMock.Setup(rgm => rgm.GetWorkOrder(It.IsAny<int>())).ReturnsAsync(new WorkOrder());
        //    _appointmentsGatewayMock.Setup(rgm => rgm.ListAppointments(It.IsAny<string>(), toDate, fromDate)).ReturnsAsync(new List<AvailableAppointmentDay>()
        //    {
        //        new AvailableAppointmentDay
        //        {
        //            Day = DayOfWeek.Friday,
        //            Id = 1,
        //            AvailableCount = 3,
        //            AvailableAppointment = new AvailableAppointment
        //            {
        //                ContractorReference = "AAA",
        //                Description = "OOF",
        //                Id = 1,
        //                EndTime = DateTime.UtcNow,
        //                StartTime = DateTime.UtcNow
        //            }
        //        }
        //    });

        //    var result = await _classUnderTest.Execute(workOrder, toDate, fromDate);

        //    result.Should().HaveCount(1);
        //}
    }
}
