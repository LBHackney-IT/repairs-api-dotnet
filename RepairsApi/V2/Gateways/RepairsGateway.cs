using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Services;
using RepairsApi.V2.Authorisation;

namespace RepairsApi.V2.Gateways
{
    public class RepairsGateway : IRepairsGateway
    {
        private readonly RepairsContext _repairsContext;
        private readonly ICurrentUserService _currentUserService;

        public RepairsGateway(RepairsContext repairsContext, ICurrentUserService currentUserService)
        {
            _repairsContext = repairsContext;
            _currentUserService = currentUserService;
        }

        public async Task<int> CreateWorkOrder(WorkOrder raiseRepair)
        {
            var entry = _repairsContext.WorkOrders.Add(raiseRepair);
            await _repairsContext.SaveChangesAsync();

            return entry.Entity.Id;
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrders(params Expression<Func<WorkOrder, bool>>[] whereExpressions)
        {
            IQueryable<WorkOrder> workOrders = _repairsContext.WorkOrders.RestrictContractor(_currentUserService)
                .Include(wo => wo.AssignedToPrimary);

            foreach (var whereExpression in whereExpressions)
            {
                workOrders = workOrders.Where(whereExpression);
            }

            return await workOrders.ToListAsync();
        }

        public async Task<WorkOrder> GetWorkOrder(int id)
        {
            var workOrder = await _repairsContext.WorkOrders
                .Include(wo => wo.AssignedToPrimary)
                .SingleOrDefaultAsync(wo => wo.Id == id);

            if (workOrder is null)
            {
                throw new ResourceNotFoundException($"Unable to locate work order {id}");
            }

            if (!UserCanAccess(workOrder)) throw new UnauthorizedAccessException($"Cannot access work order id {id}");

            return workOrder;
        }

        private bool UserCanAccess(WorkOrder workOrder)
        {
            if (_currentUserService.HasGroup(UserGroups.AGENT) ||
                _currentUserService.HasGroup(UserGroups.CONTRACT_MANAGER)) return true;

            if (_currentUserService.TryGetContractor(out string contractor))
            {
                return workOrder.AssignedToPrimary.ContractorReference == contractor;
            }

            return false;
        }

        public async Task<IEnumerable<WorkElement>> GetWorkElementsForWorkOrder(WorkOrder workOrder)
        {
            return await GetWorkElementsForWorkOrder(workOrder.Id);
        }

        public async Task<IEnumerable<WorkElement>> GetWorkElementsForWorkOrder(int id)
        {
            IQueryable<List<WorkElement>> elements =
                from wo in _repairsContext.WorkOrders.RestrictContractor(_currentUserService)
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

        public Task SaveChangesAsync()
        {
            return _repairsContext.SaveChangesAsync();
        }
    }

    public static class WorkOrderExtensions
    {
        public static IQueryable<WorkOrder> RestrictContractor(this IQueryable<WorkOrder> source, ICurrentUserService userService)
        {
            if (userService.HasGroup(UserGroups.AGENT) ||
                userService.HasGroup(UserGroups.CONTRACT_MANAGER)) return source;

            if (userService.TryGetContractor(out string contractor))
            {
                return source.Where(wo => wo.AssignedToPrimary.ContractorReference == contractor);
            }

            throw new UnauthorizedAccessException("Cannot access work orders");
        }
    }
}
