using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepairsApi.V2.Gateways;

namespace RepairsApi.V2.Generated.DRS.BackgroundService
{
    public class DrsBackgroundService : IDrsBackgroundService
    {
        private readonly ILogger<DrsBackgroundService> _logger;
        private readonly IAppointmentsGateway _appointmentsGateway;

        public DrsBackgroundService(ILogger<DrsBackgroundService> logger, IAppointmentsGateway appointmentsGateway)
        {
            _logger = logger;
            _appointmentsGateway = appointmentsGateway;
        }

        public async Task<string> ConfirmBooking(bookingConfirmation bookingConfirmation)
        {
            var serialisedBookings = JsonConvert.SerializeObject(bookingConfirmation);
            Console.WriteLine(serialisedBookings);
            _logger.LogInformation(serialisedBookings);

            await _appointmentsGateway.CreateTimedBooking(
                (int) bookingConfirmation.primaryOrderNumber,
                bookingConfirmation.planningWindowStart,
                bookingConfirmation.planningWindowEnd
            );

            return serialisedBookings;
        }
    }
}
