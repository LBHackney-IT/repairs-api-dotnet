using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    public interface IListAppointmentsUseCase
    {
        Task<object> Execute(string workOrderReference);
    }

    public class ListAppointmentsUseCase : IListAppointmentsUseCase
    {
        private readonly IAppointmentsGateway _appointmentsGateway;

        public ListAppointmentsUseCase(IAppointmentsGateway appointmentsGateway)
        {
            _appointmentsGateway = appointmentsGateway;
        }

        public Task<object> Execute(string workOrderReference)
        {
            return 
        }
    }
}
