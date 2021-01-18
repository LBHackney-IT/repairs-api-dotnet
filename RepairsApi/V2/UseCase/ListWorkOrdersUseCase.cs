using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class ListWorkOrdersUseCase : IListWorkOrdersUseCase
    {
        private readonly IRepairsGateway _repairsGateway;

        public ListWorkOrdersUseCase(IRepairsGateway repairsGateway)
        {
            _repairsGateway = repairsGateway;
        }

        public async Task<IEnumerable<WorkOrderListItem>> Execute(WorkOrderSearchParamters searchParamters)
        {
            IEnumerable<WorkOrder> workOrders;

            if (string.IsNullOrWhiteSpace(searchParamters.PropertyReference))
            {
                workOrders = await _repairsGateway.GetWorkOrders();
            }
            else
            {
                workOrders = await _repairsGateway.GetWorkOrders(wo => wo.Site.PropertyClass.Any(pc => pc.PropertyReference == searchParamters.PropertyReference));
            }

            return workOrders.Select(wo => wo.ToListItem())
                .OrderByDescending(wo => wo.DateRaised)
                .ToList();
        }
    }
}
