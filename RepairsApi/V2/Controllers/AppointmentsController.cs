using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Generated;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.UseCase.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using RepairsApi.V2.Authorisation;
using Microsoft.AspNetCore.Authorization;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/appointments")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class AppointmentsController : BaseController
    {
        private readonly IListAppointmentsUseCase _listAppointmentsUseCase;
        private readonly ICreateAppointmentUseCase _createAppointmentUseCase;

        public AppointmentsController(IListAppointmentsUseCase listAppointmentsUseCase,
            ICreateAppointmentUseCase createAppointmentUseCase)
        {
            _listAppointmentsUseCase = listAppointmentsUseCase;
            _createAppointmentUseCase = createAppointmentUseCase;
        }

        /// <summary>
        /// Returns A List of available appointments for an existing work order
        /// </summary>
        /// <param name="workOrderReference"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AppointmentDayViewModel>), 200)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = SecurityGroup.AGENT)]
        public async Task<IActionResult> ListAppointments([FromQuery][Required] int workOrderReference, string fromDate, string toDate)
        {
            try
            {
                var now = DateTime.UtcNow;
                DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                DateTime? parsedFromDate = ParseDate(fromDate);
                DateTime? parsedToDate = ParseDate(toDate);
                return base.Ok(await _listAppointmentsUseCase.Execute(workOrderReference, parsedFromDate ?? startOfMonth, parsedToDate ?? endOfMonth));
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString)) return null;

            return DateTime.ParseExact(dateString, DateConstants.DATEFORMAT, null);
        }

        /// <summary>
        /// Creates an appointment
        /// </summary>
        /// <param name="appointmentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [Authorize(Roles = SecurityGroup.AGENT)]
        public async Task<IActionResult> CreateAppointment([FromBody] RequestAppointment appointmentRequest)
        {
            try
            {
                var appointmentId = appointmentRequest.AppointmentReference.ID;
                var workOrderId = int.Parse(appointmentRequest.WorkOrderReference.ID);
                await _createAppointmentUseCase.Execute(appointmentId, workOrderId);
                return Ok();
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid Id formats. they must be integers");
            }
        }
    }
}
