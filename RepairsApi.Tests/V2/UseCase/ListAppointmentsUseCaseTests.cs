using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
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

        [Test]
        public async Task GroupList()
        {
            DateTime toDate = DateTime.UtcNow;
            DateTime fromDate = DateTime.UtcNow;
            int workOrder = 0;
            _repairGatewayMock.Setup(rgm => rgm.GetWorkOrder(It.IsAny<int>())).ReturnsAsync(new WorkOrder());
            _appointmentsGatewayMock.Setup(rgm => rgm.ListAppointments(It.IsAny<string>(), toDate, fromDate)).ReturnsAsync(new List<AppointmentDetails>()
            {
                new AppointmentDetails
                {
                    Date = DateTime.UtcNow.Date,
                    Description = "description",
                    Start = new DateTime().AddHours(9),
                    End = new DateTime().AddHours(12),
                    Id = 12
                },
                new AppointmentDetails
                {
                    Date = DateTime.UtcNow.Date,
                    Description = "other description",
                    Start = new DateTime().AddHours(13),
                    End = new DateTime().AddHours(17),
                    Id = 13
                },
                new AppointmentDetails
                {
                    Date = DateTime.UtcNow.Date.AddDays(1),
                    Description = "other description",
                    Start = new DateTime().AddHours(13),
                    End = new DateTime().AddHours(17),
                    Id = 14
                }
            });

            var result = await _classUnderTest.Execute(workOrder, toDate, fromDate);

            result.Should().HaveCount(2);
            result.First().Slots.Should().HaveCount(2);
        }

        [Test]
        public async Task EncodesDateIntoReference()
        {
            DateTime toDate = DateTime.UtcNow;
            DateTime fromDate = DateTime.UtcNow;
            int workOrder = 0;
            _repairGatewayMock.Setup(rgm => rgm.GetWorkOrder(It.IsAny<int>())).ReturnsAsync(new WorkOrder());
            var expectedAppointment = new AppointmentDetails
            {
                Date = DateTime.UtcNow.Date,
                Description = "description",
                Start = new DateTime().AddHours(9),
                End = new DateTime().AddHours(12),
                Id = 12
            };
            _appointmentsGatewayMock.Setup(rgm => rgm.ListAppointments(It.IsAny<string>(), toDate, fromDate))
                .ReturnsAsync(new List<AppointmentDetails> { expectedAppointment });

            var result = await _classUnderTest.Execute(workOrder, toDate, fromDate);

            result.Should().HaveCount(1);
            var appointment = result.First();
            var refArray = appointment.Slots.First().Reference.Split('/', 2);
            var slotId = int.Parse(refArray[0]);
            var slotDate = DateTime.ParseExact(refArray[1], DateExtensions.DATEFORMAT, null);
            slotId.Should().Be(expectedAppointment.Id);
            slotDate.Should().Be(expectedAppointment.Date);

        }
    }
}
