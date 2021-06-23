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
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using RepairsApi.V2.Services.DRS.BackgroundService;
using V2_Generated_DRS;

namespace RepairsApi.Tests.V2.Services.BackgroundService
{
    public class DrsBackgroundServiceTests
    {
        private Mock<ILogger<DrsBackgroundService>> _loggerMock;
        private Mock<IAppointmentsGateway> _appointmentsGatewayMock;
        private DrsBackgroundService _classUnderTest;
        private Mock<IDrsService> _drsServiceMock;
        private Mock<IOperativesGateway> _operativesGatewayMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<DrsBackgroundService>>();
            _appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            _drsServiceMock = new Mock<IDrsService>();
            _operativesGatewayMock = new Mock<IOperativesGateway>();
            _classUnderTest = new DrsBackgroundService(
                _loggerMock.Object,
                _appointmentsGatewayMock.Object,
                _drsServiceMock.Object,
                _operativesGatewayMock.Object
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

            _operativesGatewayMock.Verify(x => x.AssignOperatives(workOrderId, operativeId));
        }

        [Test]
        public async Task DoesNotAssignWhenOrderHasNoResource()
        {
            const int workOrderId = 1234;
            const int operativeId = 5678;
            const string operativePayrollId = "Z123";
            var bookingConfirmation = CreateBookingConfirmation(workOrderId);
            ReturnsWorkOrder(workOrderId);
            _operativesGatewayMock.Setup(x => x.GetAsync(operativePayrollId))
                .ReturnsAsync(new Operative
                {
                    Id = operativeId
                });

            await _classUnderTest.ConfirmBooking(bookingConfirmation);

            _operativesGatewayMock.Verify(x => x.AssignOperatives(It.IsAny<int>(), It.IsAny<int[]>()), Times.Never);
        }

        [Test]
        public async Task ThrowsAndLogsWhenOrderNotFound()
        {
            const int workOrderId = 1234;
            const int operativeId = 5678;
            const string operativePayrollId = "Z123";
            var bookingConfirmation = CreateBookingConfirmation(workOrderId);
            _operativesGatewayMock.Setup(x => x.GetAsync(operativePayrollId))
                .ReturnsAsync(new Operative
                {
                    Id = operativeId
                });

            Func<Task> testFunc = async () => await _classUnderTest.ConfirmBooking(bookingConfirmation);

            await testFunc.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage(Resources.WorkOrderNotFound);
            _loggerMock.VerifyLog(LogLevel.Error);
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
