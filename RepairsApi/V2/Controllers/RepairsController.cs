using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Boundary;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Generated;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using RepairsApi.V2.Authorisation;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/repairs")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class RepairsController : Controller
    {
        private readonly ICreateWorkOrderUseCase _createWorkOrderUseCase;
        private readonly IListWorkOrdersUseCase _listWorkOrdersUseCase;
        private readonly ICompleteWorkOrderUseCase _completeWorkOrderUseCase;
        private readonly IUpdateJobStatusUseCase _updateJobStatusUseCase;
        private readonly IGetWorkOrderUseCase _getWorkOrderUseCase;
        private readonly IListWorkOrderTasksUseCase _listWorkOrderTasksUseCase;
        private readonly IListWorkOrderNotesUseCase _listWorkOrderNotesUseCase;

        public RepairsController(
            ICreateWorkOrderUseCase createWorkOrderUseCase,
            IListWorkOrdersUseCase listWorkOrdersUseCase,
            ICompleteWorkOrderUseCase completeWorkOrderUseCase,
            IUpdateJobStatusUseCase updateJobStatusUseCase,
            IGetWorkOrderUseCase getWorkOrderUseCase,
            IListWorkOrderTasksUseCase listWorkOrderTasksUseCase,
            IListWorkOrderNotesUseCase listWorkOrderNotesUseCase)
        {
            _createWorkOrderUseCase = createWorkOrderUseCase;
            _listWorkOrdersUseCase = listWorkOrdersUseCase;
            _completeWorkOrderUseCase = completeWorkOrderUseCase;
            _updateJobStatusUseCase = updateJobStatusUseCase;
            _getWorkOrderUseCase = getWorkOrderUseCase;
            _listWorkOrderTasksUseCase = listWorkOrderTasksUseCase;
            _listWorkOrderNotesUseCase = listWorkOrderNotesUseCase;
        }

        /// <summary>
        /// Raise a repair (creates a work order)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = SecurityGroup.AGENT)]
        public async Task<IActionResult> RaiseRepair([FromBody] RaiseRepair request)
        {
            try
            {
                var result = await _createWorkOrderUseCase.Execute(request.ToDb());
                return Ok(result);
            }
            catch (NotSupportedException e)
            {
                return BadRequest(e.Message);
            }
        }


        /// <summary>
        /// Schedule a repair (creates a work order)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("schedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = SecurityGroup.AGENT)]
        public async Task<IActionResult> ScheduleRepair([FromBody] ScheduleRepair request)
        {
            try
            {
                var result = await _createWorkOrderUseCase.Execute(request.ToDb());
                return Ok(result);
            }
            catch (NotSupportedException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Returns a paginated list of work orders
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WorkOrderListItem>), 200)]
        public async Task<IActionResult> GetList([FromQuery] WorkOrderSearchParameters parameters)
        {
            return Ok(await _listWorkOrdersUseCase.Execute(parameters));
        }

        /// <summary>
        /// Returns a work order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(WorkOrderResponse), 200)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get(int id)
        {
            WorkOrderResponse workOrderResponse;

            try
            {
                workOrderResponse = await _getWorkOrderUseCase.Execute(id);
                return Ok(workOrderResponse);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Complete a work order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/v2/workOrderComplete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> WorkOrderComplete([FromBody] WorkOrderComplete request)
        {
            await _completeWorkOrderUseCase.Execute(request);
            return Ok();
        }

        /// <summary>
        /// Post a job status update
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/v2/jobStatusUpdate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> JobStatusUpdate([FromBody] JobStatusUpdate request)
        {
            await _updateJobStatusUseCase.Execute(request);
            return Ok();
        }

        /// <summary>
        /// Gets a list of tasks for a given work order id
        /// </summary>
        /// <param name="id">work order id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/tasks")]
        [ProducesResponseType(typeof(IEnumerable<WorkOrderItemViewModel>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ListWorkOrderTasks(int id)
        {
            var result = await _listWorkOrderTasksUseCase.Execute(id);
            return Ok(result.ToResponse());
        }

        /// <summary>
        /// Gets a list of notes for a given work order id
        /// </summary>
        /// <param name="id">work order id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/notes")]
        [ProducesResponseType(typeof(IEnumerable<NoteListItem>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ListWorkOrderNotes(int id)
        {
            var result = await _listWorkOrderNotesUseCase.Execute(id);
            return Ok(result);
        }
    }

}
