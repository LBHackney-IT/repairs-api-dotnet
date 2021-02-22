using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;

namespace RepairsApi.V2.Gateways
{
    public class WorkOrderCompletionGateway : IWorkOrderCompletionGateway
    {
        private readonly RepairsContext _repairsContext;
        private readonly ICurrentUserService _currentUserService;

        public WorkOrderCompletionGateway(RepairsContext repairsContext, ICurrentUserService currentUserService)
        {
            _repairsContext = repairsContext;
            _currentUserService = currentUserService;
        }

        public async Task<bool> IsWorkOrderCompleted(int id)
        {
            return await _repairsContext.WorkOrderCompletes.AnyAsync(woc => woc.WorkOrder.Id == id);
        }

        public async Task<int> CreateWorkOrderCompletion(WorkOrderComplete completion)
        {
            var user = _currentUserService.GetUser();
            completion.JobStatusUpdates.ForEach(jsu =>
            {
                jsu.AuthorName = user.Name();
                jsu.AuthorEmail = user.Email();
            });

            _repairsContext.WorkOrderCompletes.Add(completion);
            await _repairsContext.SaveChangesAsync();

            return completion.Id;
        }
    }
}
