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
        public async Task<IActionResult> WorkOrderComplete([FromBody] WorkOrderComplete request)
        {
            try
            {
                var result = await _completeWorkOrderUseCase.Execute(request);
                return result ? (IActionResult) Ok() : BadRequest();
            }
            catch (NotSupportedException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Post a job status update
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/v2/jobStatusUpdate")]
        public async Task<IActionResult> JobStatusUpdate([FromBody] JobStatusUpdate request)
        {
            try
            {
                var result = await _updateJobStatusUseCase.Execute(request);
                return result ? (IActionResult) Ok() : BadRequest("No WorkOrder was found with the provided ID.");
            }
            catch (NotSupportedException e)
            {
                return BadRequest(e.Message);
            }
            catch (ResourceNotFoundException e)
            {
                return NotFound(e.Message);
            }
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
            try
            {
                var result = await _listWorkOrderTasksUseCase.Execute(id);
                return Ok(result.ToResponse());
            }
            catch (NotSupportedException e)
            {
                return BadRequest(e.Message);
            }
            catch (ResourceNotFoundException e)
            {
                return NotFound(e.Message);
            }
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
            try
            {
                var result = await _listWorkOrderNotesUseCase.Execute(id);
                return Ok(result);
            }
            catch (ResourceNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }

}
