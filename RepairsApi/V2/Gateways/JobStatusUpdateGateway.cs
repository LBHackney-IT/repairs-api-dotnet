using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public class JobStatusUpdateGateway : IJobStatusUpdateGateway
    {
        private readonly RepairsContext _repairsContext;

        public JobStatusUpdateGateway(RepairsContext repairsContext)
        {
            _repairsContext = repairsContext;
        }

        public async Task<int> CreateJobStatusUpdate(JobStatusUpdate update)
        {
            _repairsContext.JobStatusUpdates.Add(update);
            await _repairsContext.SaveChangesAsync();

            return update.Id;
        }
    }
}
