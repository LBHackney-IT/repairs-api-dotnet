using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
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

        public IList<WorkOrderListItem> Execute()
        {
            var workOrders = _repairsGateway.GetWorkOrders();

            return workOrders.Select(wo => wo.ToResponse())
                .OrderByDescending(wo => wo.DateRaised)
                .ToList();
        }
    }
}
