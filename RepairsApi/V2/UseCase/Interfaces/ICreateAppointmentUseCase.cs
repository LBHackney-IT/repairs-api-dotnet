using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface ICreateAppointmentUseCase
    {
        Task Execute(string appointmentRef, int workOrderId);
    }
}
