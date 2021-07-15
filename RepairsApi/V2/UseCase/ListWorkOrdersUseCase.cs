using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            var workOrders = await _repairsGateway.GetWorkOrders(filter);

            workOrders = EnsureSorting(searchParameters, workOrders);

            return workOrders
                .Skip((searchParameters.PageNumber - 1) * searchParameters.PageSize)
                .Take(searchParameters.PageSize)
                .Select(wo => wo.ToListItem());
        }

        private static IEnumerable<WorkOrder> EnsureSorting(WorkOrderSearchParameters searchParameters, IEnumerable<WorkOrder> workOrders)
        {
            var statusOrders = new[] {
                WorkStatusCode.Open,
                WorkStatusCode.VariationPendingApproval,
                WorkStatusCode.Canceled,
                WorkStatusCode.Complete
            };

            if (!IsSortSpecified(searchParameters))
            {
                workOrders = AddDefaultSort(workOrders, statusOrders);
            }

            return workOrders;
        }

        private static IEnumerable<WorkOrder> AddDefaultSort(IEnumerable<WorkOrder> workOrders, WorkStatusCode[] statusOrder)
        {
            workOrders = workOrders
                .OrderBy(wo => Array.IndexOf(statusOrder, wo.StatusCode))
                .ThenByDescending(wo => wo.DateRaised);
            return workOrders;
        }

        private static bool IsSortSpecified(WorkOrderSearchParameters searchParameters)
        {
            return !string.IsNullOrWhiteSpace(searchParameters.Sort);
        }
    }
}
