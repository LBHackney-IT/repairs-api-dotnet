using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated.DRS.BackgroundService;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services.DRS.BackgroundService;

namespace RepairsApi.Tests.V2.Services.BackgroundService
{
    public class DrsBackgroundServiceTests
    {
        private Mock<ILogger<DrsBackgroundService>> _loggerMock;
        private Mock<IAppointmentsGateway> _appointmentsGatewayMock;
        private DrsBackgroundService _classUnderTest;
        private Mock<IRepairsGateway> _repairsGatewayMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<DrsBackgroundService>>();
            _appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _classUnderTest = new DrsBackgroundService(
                _loggerMock.Object,
                _appointmentsGatewayMock.Object,
                _repairsGatewayMock.Object
            );
        }

        [Test]
        public async Task LogsDetails()
        {
            var bookingConfirmation = new bookingConfirmation();

            await _classUnderTest.ConfirmBooking(bookingConfirmation);

            _loggerMock.VerifyLog(LogLevel.Information);
        }

        [Test]
        public async Task AddAppointmentAtCorrectTime()
        {
            const int workOrderId = 1234;
            var bookingConfirmation = new bookingConfirmation
            {
                primaryOrderNumber = workOrderId,
                planningWindowStart = DateTime.UtcNow,
                planningWindowEnd = DateTime.UtcNow.AddHours(5)
            };

            var result = await _classUnderTest.ConfirmBooking(bookingConfirmation);

            _appointmentsGatewayMock.Verify(x => x.CreateTimedBooking(
                workOrderId,
                bookingConfirmation.planningWindowStart,
                bookingConfirmation.planningWindowEnd
            ));
            result.Should().Be(Resources.DrsBackgroundService_AddedBooking);
        }

        [Test]
        public async Task UpdatesExistingAppointment()
        {
            var appointmentGenerator = new Generator<AppointmentDetails>()
                .AddDefaultGenerators();
            const int workOrderId = 1234;
            var bookingConfirmation = new bookingConfirmation
            {
                primaryOrderNumber = workOrderId,
                planningWindowStart = DateTime.UtcNow,
                planningWindowEnd = DateTime.UtcNow.AddHours(5)
            };
            var existingAppointment = appointmentGenerator.Generate();
            _appointmentsGatewayMock.Setup(x => x.GetAppointment(workOrderId))
                .ReturnsAsync(existingAppointment);

            var result = await _classUnderTest.ConfirmBooking(bookingConfirmation);

            _appointmentsGatewayMock.Verify(x => x.CreateTimedBooking(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
            existingAppointment.Date.Should().Be(bookingConfirmation.planningWindowStart.Date);
            existingAppointment.Start.Should().Be(bookingConfirmation.planningWindowStart);
            existingAppointment.End.Should().Be(bookingConfirmation.planningWindowEnd);
            result.Should().Be(Resources.DrsBackgroundServiceTests_UpdatedBooking);
        }

        [Test]
        public async Task ThrowsIfWorkOrderNotFound()
        {
            const int workOrderId = 1234;
            ResourceNotFoundException expectedException = new ResourceNotFoundException("test");
            _repairsGatewayMock.Setup(x => x.GetWorkOrder(workOrderId))
                .ThrowsAsync(expectedException);

            var bookingConfirmation = new bookingConfirmation
            {
                primaryOrderNumber = workOrderId,
                planningWindowStart = DateTime.UtcNow,
                planningWindowEnd = DateTime.UtcNow.AddHours(5)
            };

            Func<Task> act = () => _classUnderTest.ConfirmBooking(bookingConfirmation);

            await act.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage(expectedException.Message);
        }
    }
}
