using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated.DRS.BackgroundService;

namespace RepairsApi.Tests.V2.Services.BackgroundService
{
    public class DrsBackgroundServiceTests
    {
        private Mock<ILogger<DrsBackgroundService>> _loggerMock;
        private Mock<IAppointmentsGateway> _appointmentsGatewayMock;
        private DrsBackgroundService _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<DrsBackgroundService>>();
            _appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            _classUnderTest = new DrsBackgroundService(
                _loggerMock.Object,
                _appointmentsGatewayMock.Object
            );
        }

        [Test]
        public void LogsDetails()
        {
            var bookingConfirmation = new bookingConfirmation();

            _classUnderTest.ConfirmBooking(bookingConfirmation);

            _loggerMock.VerifyLog(LogLevel.Information);
        }

        [Test]
        public void AddAppointmentAtCorrectTime()
        {
            const int workOrderId = 1234;
            var bookingConfirmation = new bookingConfirmation
            {
                primaryOrderNumber = workOrderId,
                planningWindowStart = DateTime.UtcNow,
                planningWindowEnd = DateTime.UtcNow.AddHours(5)
            };

            _classUnderTest.ConfirmBooking(bookingConfirmation);

            _appointmentsGatewayMock.Verify(x => x.CreateTimedBooking(
                workOrderId,
                bookingConfirmation.planningWindowStart,
                bookingConfirmation.planningWindowEnd
            ));
        }
    }
}
