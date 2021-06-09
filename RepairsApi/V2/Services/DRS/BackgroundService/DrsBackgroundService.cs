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
        private readonly IRepairsGateway _repairsGateway;

        public DrsBackgroundService(
            ILogger<DrsBackgroundService> logger,
            IAppointmentsGateway appointmentsGateway,
            IRepairsGateway repairsGateway
            )
        {
            _logger = logger;
            _appointmentsGateway = appointmentsGateway;
            _repairsGateway = repairsGateway;
        }

        public async Task<string> ConfirmBooking(bookingConfirmation bookingConfirmation)
        {
            var serialisedBookings = JsonConvert.SerializeObject(bookingConfirmation);
            Console.WriteLine(serialisedBookings);
            _logger.LogInformation(serialisedBookings);
            var workOrderId = (int) bookingConfirmation.primaryOrderNumber;

            await _repairsGateway.GetWorkOrder(workOrderId);

            var existingAppointment = await _appointmentsGateway.GetAppointment(workOrderId);

            if (existingAppointment != null)
            {
                existingAppointment.Date = bookingConfirmation.planningWindowStart.Date;
                existingAppointment.Start = bookingConfirmation.planningWindowStart;
                existingAppointment.End = bookingConfirmation.planningWindowEnd;
                return Resources.DrsBackgroundServiceTests_UpdatedBooking;
            }

            await _appointmentsGateway.CreateTimedBooking(
                workOrderId,
                bookingConfirmation.planningWindowStart,
                bookingConfirmation.planningWindowEnd
            );

            return Resources.DrsBackgroundService_AddedBooking;
        }
    }
}
