using System.Threading.Tasks;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;

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
    }
}
