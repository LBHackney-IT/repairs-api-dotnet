using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    public interface ICreateAppointmentUseCase
    {
        Task Execute(string appointmentId, string workOrderId);
    }

    public class CreateAppointmentUseCase : ICreateAppointmentUseCase
    {
        private readonly IAppointmentsGateway _appointmentsGateway;

        public CreateAppointmentUseCase(IAppointmentsGateway appointmentsGateway)
        {
            _appointmentsGateway = appointmentsGateway;
        }

        public async Task Execute(string appointmentId, string workOrderId)
        {
            await _appointmentsGateway.Create(appointmentId, workOrderId);
        }
    }
}
