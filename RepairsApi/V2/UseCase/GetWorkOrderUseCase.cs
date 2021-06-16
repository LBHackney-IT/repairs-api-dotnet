using RepairsApi.V2.Boundary;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Services;

namespace RepairsApi.V2.UseCase
{
    public class GetWorkOrderUseCase : IGetWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IAppointmentsGateway _appointmentGateway;
        private readonly IOptions<DrsOptions> _drsOptions;

        public GetWorkOrderUseCase(IRepairsGateway repairsGateway, IAppointmentsGateway appointmentGateway, IOptions<DrsOptions> drsOptions)
        {
            _repairsGateway = repairsGateway;
            _appointmentGateway = appointmentGateway;
            _drsOptions = drsOptions;
        }

        public async Task<WorkOrderResponse> Execute(int id)
        {
            WorkOrder workOrder = await _repairsGateway.GetWorkOrder(id);

            var appointment = await _appointmentGateway.GetAppointment(workOrder.Id);

            return workOrder.ToResponse(appointment, _drsOptions.Value.ManagementAddress);
        }
    }
}
