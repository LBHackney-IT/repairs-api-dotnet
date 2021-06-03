using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RepairsApi.V2.Generated.DRS.BackgroundService
{
    public class DrsBackgroundService : IDrsBackgroundService
    {
        private readonly ILogger<DrsBackgroundService> _logger;

        public DrsBackgroundService(ILogger<DrsBackgroundService> logger)
        {
            _logger = logger;
        }

        public string ConfirmBooking(bookingConfirmation bookingConfirmation)
        {
            var serialisedBookings = JsonConvert.SerializeObject(bookingConfirmation);
            Console.WriteLine(serialisedBookings);
            _logger.LogInformation(serialisedBookings);

            return serialisedBookings;
        }
    }
}
