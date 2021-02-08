using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class ListAppointmentsUseCase : IListAppointmentsUseCase
    {
        private readonly IRepairsGateway _repairsGateway;

        public ListAppointmentsUseCase(IRepairsGateway repairsGateway)
        {
            _repairsGateway = repairsGateway;
        }

        public async Task Execute(int workOrderId, DateTime fromDate, DateTime toDate)
        {
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            if (workOrder is null) throw new ResourceNotFoundException("Work Order does not exist");
        }
    }
}
