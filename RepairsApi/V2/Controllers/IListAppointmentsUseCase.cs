using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    public interface IListAppointmentsUseCase
    {
        Task<IEnumerable<AppointmentDayViewModel>> Execute(int workOrder, DateTime fromDate, DateTime toDate);
    }
}
