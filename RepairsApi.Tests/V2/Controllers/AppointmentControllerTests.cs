using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.UseCase.Interfaces;
using RepairsApi.V2;

namespace RepairsApi.Tests.V2.Controllers
{
    public class AppointmentControllerTests : ControllerTests
    {
        private AppointmentsController _classUnderTest;
        private Mock<IListAppointmentsUseCase> _listAppointmentsMock;
        private Mock<ICreateAppointmentUseCase> _createAppointmentMock;

        [SetUp]
        public void Setup()
        {
            _listAppointmentsMock = new Mock<IListAppointmentsUseCase>();
            _createAppointmentMock = new Mock<ICreateAppointmentUseCase>();
            _classUnderTest = new AppointmentsController(_listAppointmentsMock.Object, _createAppointmentMock.Object);
        }

        [Test]
        public async Task ListReturns404ForNotFound()
        {
            _listAppointmentsMock.Setup(lam => lam.Execute(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new ResourceNotFoundException());
            var result = await _classUnderTest.ListAppointments(1, DateTime.UtcNow.AddDays(-1).ToString(DateConstants.DATEFORMAT), DateTime.UtcNow.AddDays(1).ToString(DateConstants.DATEFORMAT));

            GetStatusCode(result).Should().Be(404);
        }

        [Test]
        public async Task ListReturns()
        {
            IEnumerable<AppointmentDayViewModel> list = new List<AppointmentDayViewModel>
            {
                new AppointmentDayViewModel
                {
                    Date = DateTime.UtcNow,
                    Slots = new List<AppointmentSlot>
                    {
                        new AppointmentSlot
                        {
                            Description = "desc",
                            End = new DateTime().AddHours(8),
                            Start = new DateTime().AddHours(12),
                            Reference = "117"
                        }
                    }
                }
            };
            _listAppointmentsMock.Setup(lam => lam.Execute(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(list);
            var result = await _classUnderTest.ListAppointments(1, DateTime.UtcNow.AddDays(-1).ToString(DateConstants.DATEFORMAT), DateTime.UtcNow.AddDays(1).ToString(DateConstants.DATEFORMAT));

            GetStatusCode(result).Should().Be(200);
            GetResultData<IEnumerable<AppointmentDayViewModel>>(result).Should().BeEquivalentTo(list);
        }

        [Test]
        public async Task Returns404ForNotFound()
        {
            _createAppointmentMock.Setup(lam => lam.Execute(It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new ResourceNotFoundException());

            var result = await _classUnderTest.CreateAppointment(new RepairsApi.V2.Generated.RequestAppointment
            {
                AppointmentReference = new RepairsApi.V2.Generated.Reference
                {
                    ID = "1"
                },
                WorkOrderReference = new RepairsApi.V2.Generated.Reference
                {
                    ID = "2"
                }
            });

            GetStatusCode(result).Should().Be(404);
        }

        [Test]
        public async Task Returns400ForNotSupported()
        {
            _createAppointmentMock.Setup(lam => lam.Execute(It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new NotSupportedException());

            var result = await _classUnderTest.CreateAppointment(new RepairsApi.V2.Generated.RequestAppointment
            {
                AppointmentReference = new RepairsApi.V2.Generated.Reference
                {
                    ID = "1"
                },
                WorkOrderReference = new RepairsApi.V2.Generated.Reference
                {
                    ID = "2"
                }
            });

            GetStatusCode(result).Should().Be(400);
        }

        [Test]
        public async Task CallsUsecaseForCreate()
        {
            const int appointmentId = 1;
            const int workOrderId = 2;
            var result = await _classUnderTest.CreateAppointment(new RepairsApi.V2.Generated.RequestAppointment
            {
                AppointmentReference = new RepairsApi.V2.Generated.Reference
                {
                    ID = appointmentId.ToString()
                },
                WorkOrderReference = new RepairsApi.V2.Generated.Reference
                {
                    ID = workOrderId.ToString()
                }
            });

            GetStatusCode(result).Should().Be(200);
            _createAppointmentMock.Verify(cam => cam.Execute(appointmentId.ToString(), workOrderId));
        }
    }
}
