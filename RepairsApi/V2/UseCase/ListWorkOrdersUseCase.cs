using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Controllers.Parameters;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class ListWorkOrdersUseCase : IListWorkOrdersUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IFilterBuilder<WorkOrderSearchParameters, WorkOrder> _filterBuilder;

        public ListWorkOrdersUseCase(IRepairsGateway repairsGateway, IFilterBuilder<WorkOrderSearchParameters, WorkOrder> filterBuilder)
        {
            _repairsGateway = repairsGateway;
            _filterBuilder = filterBuilder;
        }

        public async Task<IEnumerable<WorkOrderListItem>> Execute(WorkOrderSearchParameters searchParameters)
        {
            var filter = _filterBuilder.BuildFilter(searchParameters);
            IEnumerable<WorkOrder> workOrders = await _repairsGateway.GetWorkOrders(filter);

            var statusOrder = new[] {
                WorkOrderStatus.InProgress,
                WorkOrderStatus.VariationPendingApproval,
                WorkOrderStatus.Cancelled,
                WorkOrderStatus.Complete,
                WorkOrderStatus.Unknown
            };

            return workOrders.Select(wo => wo.ToListItem())
                .OrderBy(wo => Array.IndexOf(statusOrder, wo.Status))
                .ThenByDescending(wo => wo.DateRaised)
                .Skip((searchParameters.PageNumber - 1) * searchParameters.PageSize)
                .Take(searchParameters.PageSize)
                .ToList();
        }
    }
}
