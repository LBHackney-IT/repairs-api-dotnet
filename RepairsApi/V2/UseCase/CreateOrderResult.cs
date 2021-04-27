
using System;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.UseCase
{
    public class CreateOrderResult
    {
        public int Id { get; }
        public WorkStatusCode StatusCode { get; }
        public string StatusCodeDescription { get; }
        public bool ExternallyManagedAppointment { get; set; }
        public Uri ExternalAppointmentManagementUrl { get; set; }

        public CreateOrderResult(int id, WorkStatusCode statusCode, string statusCodeDescription)
        {
            Id = id;
            StatusCode = statusCode;
            StatusCodeDescription = statusCodeDescription;
            ExternallyManagedAppointment = false;
        }
    }
}
