using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    public interface IAppointmentsGateway
    {
        Task Create(string appointmentId, string workOrderId);
    }
}