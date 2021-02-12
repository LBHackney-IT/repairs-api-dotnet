using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IListAppointmentsUseCase
    {
        Task<IEnumerable<AppointmentDayViewModel>> Execute(int workOrder, DateTime fromDate, DateTime toDate);
    }
}
