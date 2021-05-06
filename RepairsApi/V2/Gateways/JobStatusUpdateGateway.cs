using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobStatusUpdateTypeCode = RepairsApi.V2.Generated.JobStatusUpdateTypeCode;

namespace RepairsApi.V2.Gateways
{
    public class JobStatusUpdateGateway : IJobStatusUpdateGateway
    {
        private readonly RepairsContext _repairsContext;
        private readonly ICurrentUserService _currentUserService;

        public JobStatusUpdateGateway(RepairsContext repairsContext, ICurrentUserService currentUserService)
        {
            _repairsContext = repairsContext;
            _currentUserService = currentUserService;
        }

        public async Task<int> CreateJobStatusUpdate(JobStatusUpdate update)
        {
            update.AuthorName = _currentUserService.GetUser().Name();
            update.AuthorEmail = _currentUserService.GetUser().Email();
            await _repairsContext.JobStatusUpdates.AddAsync(update);
            await _repairsContext.SaveChangesAsync();

            return update.Id;
        }

        public async Task<JobStatusUpdate> GetOutstandingVariation(int workOrderId)
        {
            var jobStatusUpdate = await _repairsContext.JobStatusUpdates.Where(s => (int) s.TypeCode == (int) JobStatusUpdateTypeCode._180)
                .Where(s => s.RelatedWorkOrder.Id == workOrderId)
                .OrderByDescending(s => s.EventTime)
                .FirstOrDefaultAsync();

            if (jobStatusUpdate is null)
            {
                throw new ResourceNotFoundException($"Unable to locate outstanding variation for work order {workOrderId}");
            }

            return jobStatusUpdate;
        }
    }
}
