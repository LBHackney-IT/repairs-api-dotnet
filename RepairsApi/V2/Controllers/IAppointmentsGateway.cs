using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    public interface IAppointmentsGateway
    {
        Task Create(int appointmentId, int workOrderId);
        Task<IEnumerable<AvailableAppointmentDay>> ListAppointments(string contractorReference, DateTime fromDate, DateTime toDate);
    }
}
