using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V1.Boundary.Response;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase.Interfaces;

namespace RepairsApi.V1.UseCase
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

            return workOrders.Select(wo => wo.ToResponse()).ToList();
        }
    }
}
