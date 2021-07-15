using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using RepairsApi.V2.Services.DRS;
using RepairsApi.V2.Services.DRS.BackgroundService;
using V2_Generated_DRS;
using JobStatusUpdate = RepairsApi.V2.Infrastructure.JobStatusUpdate;

namespace RepairsApi.Tests.V2.Services.BackgroundService
{
    public class DrsBackgroundServiceTests
    {
        private Mock<ILogger<DrsBackgroundService>> _loggerMock;
        private Mock<IAppointmentsGateway> _appointmentsGatewayMock;
        private DrsBackgroundService _classUnderTest;
        private Mock<IDrsService> _drsServiceMock;
        private Mock<IOperativesGateway> _operativesGatewayMock;
        private Mock<IJobStatusUpdateGateway> _jobStatusUpdateGatewayMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<DrsBackgroundService>>();
            _appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            _drsServiceMock = new Mock<IDrsService>();
            _operativesGatewayMock = new Mock<IOperativesGateway>();
            _jobStatusUpdateGatewayMock = new Mock<IJobStatusUpdateGateway>();
            _classUnderTest = new DrsBackgroundService(
                _loggerMock.Object,
                _appointmentsGatewayMock.Object,
                _drsServiceMock.Object,
                _operativesGatewayMock.Object,
                _jobStatusUpdateGatewayMock.Object
            );
        }

        [Test]
        public async Task LogsDetails()
        {
            const int workOrderId = 1234;
            ReturnsWorkOrder(workOrderId);
            var bookingConfirmation = CreateBookingConfirmation(workOrderId);

            await _classUnderTest.ConfirmBooking(bookingConfirmation);

            _loggerMock.VerifyLog(LogLevel.Information);
        }

        [Test]
        public async Task AddAppointmentAtCorrectTime()
        {
            const int workOrderId = 1234;
            ReturnsWorkOrder(workOrderId);
            var bookingConfirmation = CreateBookingConfirmation(workOrderId);

            var result = await _classUnderTest.ConfirmBooking(bookingConfirmation);

            _appointmentsGatewayMock.Verify(x => x.SetTimedBooking(
                workOrderId,
                bookingConfirmation.planningWindowStart,
                bookingConfirmation.planningWindowEnd
            ));
            result.Should().Be(Resources.DrsBackgroundService_BookingAccepted);
        }

        [Test]
        public async Task UpdatesAssignedOperative()
        {
            const int workOrderId = 1234;
            const int operativeId = 5678;
            const string operativePayrollId = "Z123";
            var bookingConfirmation = CreateBookingConfirmation(workOrderId);
            ReturnsWorkOrder(workOrderId, operativePayrollId);
            _operativesGatewayMock.Setup(x => x.GetAsync(operativePayrollId))
                .ReturnsAsync(new Operative
                {
                    Id = operativeId
                });

            await _classUnderTest.ConfirmBooking(bookingConfirmation);

            _drsServiceMock.Verify(x => x.UpdateWorkOrderDetails(workOrderId));
        }

        [Test]
        public async Task AddsNote()
        {
            const int workOrderId = 1234;
            ReturnsWorkOrder(workOrderId);
            var bookingConfirmation = CreateBookingConfirmation(workOrderId);

            await _classUnderTest.ConfirmBooking(bookingConfirmation);

            var expectedComment = $"DRS: Appointment has been updated in DRS to {bookingConfirmation.planningWindowStart} - {bookingConfirmation.planningWindowEnd}";
            _jobStatusUpdateGatewayMock.Verify(x => x.CreateJobStatusUpdate(It.Is<JobStatusUpdate>(update =>
                update.RelatedWorkOrderId == workOrderId &&
                update.TypeCode == JobStatusUpdateTypeCode._0 &&
                update.OtherType == CustomJobStatusUpdates.AddNote &&
                update.Comments == expectedComment &&
                update.EventTime == DrsHelpers.ConvertFromDrsTimeZone(bookingConfirmation.changedDate)
            )));
        }

        private static order CreateDrsOrder(params string[] payrollIds)
        {
            return new order
            {
                theBookings = new[]
                {
                    new booking
                    {
                        theResources = payrollIds.Select(ori =>
                            new resource
                            {
                                externalResourceCode = ori
                            }).ToArray()
                    }
                }
            };
        }

        private static bookingConfirmation CreateBookingConfirmation(int workOrderId)
        {
            var bookingConfirmation = new bookingConfirmation
            {
                changedDate = DateTime.UtcNow,
                primaryOrderNumber = (uint) workOrderId,
                planningWindowStart = DateTime.UtcNow,
                planningWindowEnd = DateTime.UtcNow.AddHours(5)
            };
            return bookingConfirmation;
        }

        private void ReturnsWorkOrder(int workOrderId, params string[] operativePayrollId)
        {
            _drsServiceMock.Setup(x => x.SelectOrder(workOrderId))
                .ReturnsAsync(CreateDrsOrder(operativePayrollId));
        }
    }
}
