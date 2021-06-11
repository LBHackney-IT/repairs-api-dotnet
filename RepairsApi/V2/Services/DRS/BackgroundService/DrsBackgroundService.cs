using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated.DRS.BackgroundService;

namespace RepairsApi.V2.Services.DRS.BackgroundService
{
    public class DrsBackgroundService : IDrsBackgroundService
    {
        private readonly ILogger<DrsBackgroundService> _logger;
        private readonly IAppointmentsGateway _appointmentsGateway;

        public DrsBackgroundService(
            ILogger<DrsBackgroundService> logger,
            IAppointmentsGateway appointmentsGateway
            )
        {
            _logger = logger;
            _appointmentsGateway = appointmentsGateway;
        }

        public async Task<string> ConfirmBooking(bookingConfirmation bookingConfirmation)
        {
            var serialisedBookings = JsonConvert.SerializeObject(bookingConfirmation);
            Console.WriteLine(serialisedBookings);
            _logger.LogInformation(serialisedBookings);
            var workOrderId = (int) bookingConfirmation.primaryOrderNumber;

            await _appointmentsGateway.SetTimedBooking(
                workOrderId,
                bookingConfirmation.planningWindowStart,
                bookingConfirmation.planningWindowEnd
            );

            return Resources.DrsBackgroundService_BookingAccepted;
        }
    }
}
