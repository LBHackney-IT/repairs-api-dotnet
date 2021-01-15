using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
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
            var entry = _repairsContext.WorkOrders.Add(raiseRepair);
            await _repairsContext.SaveChangesAsync();

            return entry.Entity.Id;
        }

        public IEnumerable<WorkOrder> GetWorkOrders()
        {
            return _repairsContext.WorkOrders.ToList();
        }

        public Task<WorkOrder> GetWorkOrder(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
