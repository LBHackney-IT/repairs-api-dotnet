using System.Threading.Tasks;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface ICreateAppointmentUseCase
    {
        Task Execute(string appointmentRef, int workOrderId);
    }

    public class CreateAppointmentUseCase : ICreateAppointmentUseCase
    {
        private readonly IAppointmentsGateway _appointmentsGateway;
        private readonly IRepairsGateway _repairsGateway;

        public CreateAppointmentUseCase(IAppointmentsGateway appointmentsGateway, IRepairsGateway repairsGateway)
        {
            _appointmentsGateway = appointmentsGateway;
            _repairsGateway = repairsGateway;
        }

        public async Task Execute(string appointmentRef, int workOrderId)
        {
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            if (workOrder is null)
            {
                throw new ResourceNotFoundException("work order does not exist");
            }

            await _appointmentsGateway.Create(appointmentRef, workOrderId);
        }
    }
}
