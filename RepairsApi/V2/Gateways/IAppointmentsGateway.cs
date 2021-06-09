using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
    public interface IAppointmentsGateway
    {
        Task CreateSlotBooking(string appointmentRef, int workOrderId);
        Task CreateTimedBooking(int workOrderId, DateTime startTime, DateTime endTime);
        Task<IEnumerable<AppointmentDetails>> ListAppointments(string contractorReference, DateTime from, DateTime toDate);
        Task<AppointmentDetails> GetAppointment(int id);
    }
}
