using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
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
        private readonly IDrsService _drsService;
        private readonly IOperativesGateway _operativesGateway;

        public DrsBackgroundService(
            ILogger<DrsBackgroundService> logger,
            IAppointmentsGateway appointmentsGateway,
            IDrsService drsService,
            IOperativesGateway operativesGateway
        )
        {
            _logger = logger;
            _appointmentsGateway = appointmentsGateway;
            _drsService = drsService;
            _operativesGateway = operativesGateway;
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

            await UpdateAssignedOperative(workOrderId);

            return Resources.DrsBackgroundService_BookingAccepted;
        }

        private async Task UpdateAssignedOperative(int workOrderId)
        {
            var order = await _drsService.SelectOrder(workOrderId);

            if (order is null)
            {
                _logger.LogError($"Unable to fetch order from DRS {workOrderId}", workOrderId);
                ThrowHelper.ThrowNotFound(Resources.WorkOrderNotFound);
            }

            var theResources = order.theBookings.First().theResources;

            if (theResources.IsNullOrEmpty()) return;

            var operativePayrollIds = theResources.Select(r => r.externalResourceCode);
            var operatives = await Task.WhenAll(operativePayrollIds.Select(i => _operativesGateway.GetAsync(i)));

            await _operativesGateway.AssignOperatives(workOrderId, operatives.Select(o => o.Id).ToArray());
        }
    }
}
