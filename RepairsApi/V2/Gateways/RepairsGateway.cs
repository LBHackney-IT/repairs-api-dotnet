using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using RepairsApi.V2.Exceptions;

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
            IQueryable<WorkOrder> workOrders = _repairsContext.WorkOrders
                .Include(wo => wo.AssignedToPrimary);

            foreach (var whereExpression in whereExpressions)
            {
                workOrders = workOrders.Where(whereExpression);
            }

            return await workOrders.ToListAsync();
        }

        public async Task<WorkOrder> GetWorkOrder(int id)
        {
            return await _repairsContext.WorkOrders
                .Include(wo => wo.AssignedToPrimary)
                .SingleOrDefaultAsync(wo => wo.Id == id);

            if (workOrder is null)
            {
                throw new ResourceNotFoundException($"Unable to locate work order {id}");
            }

            return workOrder;
        }

        public async Task<IEnumerable<WorkElement>> GetWorkElementsForWorkOrder(WorkOrder workOrder)
        {
            return await GetWorkElementsForWorkOrder(workOrder.Id);
        }

        public async Task<IEnumerable<WorkElement>> GetWorkElementsForWorkOrder(int id)
        {
            IQueryable<List<WorkElement>> elements =
                from wo in _repairsContext.WorkOrders
                where wo.Id == id
                select wo.WorkElements;

            return await elements.SingleOrDefaultAsync();
        }

        public async Task UpdateWorkOrderStatus(int workOrderId, WorkStatusCode newCode)
        {
            var order = await GetWorkOrder(workOrderId);
            order.StatusCode = newCode;
            await _repairsContext.SaveChangesAsync();
        }
    }
}
