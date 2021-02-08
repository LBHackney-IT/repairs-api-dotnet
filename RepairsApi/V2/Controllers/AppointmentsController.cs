using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Generated;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AppointmentDayViewModel>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ListAppointments([FromQuery]string workOrderReference)
        {
            try
            {
                var result = await _listAppointmentsUseCase.Execute(workOrderReference);
                return Ok(result); // TODO to response
            } catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
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
        public async Task<IActionResult> CreateAppointment([FromBody] RequestAppointment appointmentRequest)
        {
            try
            {
                await _createAppointmentUseCase.Execute(appointmentRequest.AppointmentReference.ID, appointmentRequest.WorkOrderReference.ID);
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
        }
    }
}
