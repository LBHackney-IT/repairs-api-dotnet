using RepairsApi.V2.Boundary;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class GetWorkOrderUseCase : IGetWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IAppointmentsGateway _appointmentGateway;

        public GetWorkOrderUseCase(IRepairsGateway repairsGateway, IAppointmentsGateway appointmentGateway)
        {
            _repairsGateway = repairsGateway;
            _appointmentGateway = appointmentGateway;
        }

        public async Task<WorkOrderResponse> Execute(int id)
        {
            WorkOrder workOrder = await _repairsGateway.GetWorkOrder(id);

            var appointment = await _appointmentGateway.GetAppointment(workOrder.Id);

            return workOrder.ToResponse(appointment);
        }
    }
}
