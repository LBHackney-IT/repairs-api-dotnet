using System.Threading.Tasks;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{

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

            workOrder.VerifyCanBookAppointment();

            await _appointmentsGateway.CreateSlotBooking(appointmentRef, workOrderId);
        }
    }
}
