using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Generated.DRS.BackgroundService;
using JobStatusUpdate = RepairsApi.V2.Infrastructure.JobStatusUpdate;

namespace RepairsApi.V2.Services.DRS.BackgroundService
{
    public class DrsBackgroundService : IDrsBackgroundService
    {
        private readonly ILogger<DrsBackgroundService> _logger;
        private readonly IAppointmentsGateway _appointmentsGateway;
        private readonly IDrsService _drsService;
        private readonly IOperativesGateway _operativesGateway;
        private readonly IJobStatusUpdateGateway _jobStatusUpdateGateway;

        public DrsBackgroundService(
            ILogger<DrsBackgroundService> logger,
            IAppointmentsGateway appointmentsGateway,
            IDrsService drsService,
            IOperativesGateway operativesGateway,
            IJobStatusUpdateGateway jobStatusUpdateGateway
        )
        {
            _logger = logger;
            _appointmentsGateway = appointmentsGateway;
            _drsService = drsService;
            _operativesGateway = operativesGateway;
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
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

            await AddAuditTrail(workOrderId, bookingConfirmation);

            return Resources.DrsBackgroundService_BookingAccepted;
        }

        private async Task AddAuditTrail(int workOrderId, bookingConfirmation bookingConfirmation)
        {
            var update = new JobStatusUpdate
            {
                EventTime = bookingConfirmation.changedDate,
                RelatedWorkOrderId = workOrderId,
                TypeCode = JobStatusUpdateTypeCode._0,
                OtherType = CustomJobStatusUpdates.AddNote,
                Comments = $"DRS: Appointment has been updated in DRS to {bookingConfirmation.planningWindowStart} - {bookingConfirmation.planningWindowEnd}"
            };

            await _jobStatusUpdateGateway.CreateJobStatusUpdate(update);
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
