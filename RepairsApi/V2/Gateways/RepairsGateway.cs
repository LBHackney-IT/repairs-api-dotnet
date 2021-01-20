using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

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

        public async Task<IEnumerable<WorkOrder>> GetWorkOrders(params Expression<Func<WorkOrder, bool>>[] whereExpressions)
        {
            IQueryable<WorkOrder> workOrders = _repairsContext.WorkOrders;

            foreach (var whereExpression in whereExpressions)
            {
                workOrders = workOrders.Where(whereExpression);
            }

            return await workOrders.ToListAsync();
        }

        public async Task<WorkOrder> GetWorkOrder(int id)
        {
            return await _repairsContext.WorkOrders.FindAsync(id);
        }

        public async Task<IEnumerable<WorkElement>> GetWorkElementsForWorkOrder(WorkOrder workOrder)
        {
            var elements =
                from wo in _repairsContext.WorkOrders
                where wo.Id == workOrder.Id
                select wo.WorkElements;

            return await elements.SingleOrDefaultAsync();
        }
    }
}
