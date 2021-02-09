using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    public interface IAppointmentsGateway
    {
        Task Create(int appointmentId, int workOrderId, DateTime appointmentDate);
        Task<IEnumerable<AppointmentListResult>> ListAppointments(string contractorReference, DateTime from, DateTime toDate);
    }
}
