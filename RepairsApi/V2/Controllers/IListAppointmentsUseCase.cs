using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    public interface IListAppointmentsUseCase
    {
        Task Execute(int workOrder, DateTime fromDate, DateTime toDate);
    }
}
