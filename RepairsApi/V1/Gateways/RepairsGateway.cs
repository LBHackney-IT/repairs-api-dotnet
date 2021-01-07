using RepairsApi.V1.Domain.Repair;
using RepairsApi.V1.Infrastructure;
using System.Threading.Tasks;
using RepairsApi.V1.Factories;
using WorkOrder = RepairsApi.V1.Domain.Repair.WorkOrder;

namespace RepairsApi.V1.Gateways
{
    public class RepairsGateway : IRepairsGateway
    {
        private readonly RepairsContext _repairsContext;

        public RepairsGateway(RepairsContext repairsContext)
        {
            _repairsContext = repairsContext;
        }

        public async Task<int> CreateWorkOrder(WorkOrder raiseRepair)
        {
            var entry = _repairsContext.WorkOrders.Add(raiseRepair.ToDb());
            await _repairsContext.SaveChangesAsync();

            return entry.Entity.Id;
        }
    }
}
