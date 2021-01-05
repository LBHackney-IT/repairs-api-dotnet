using RepairsApi.V1.Domain.Repair;
using RepairsApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public class RepairsGateway : IRepairsGateway
    {
        private readonly RepairsContext _repairsContext;

        public RepairsGateway(RepairsContext repairsContext)
        {
            _repairsContext = repairsContext;
        }

        public Task CreateWorkOrder(WorkOrder raiseRepair)
        {
            return Task.CompletedTask;
        }
    }
}
