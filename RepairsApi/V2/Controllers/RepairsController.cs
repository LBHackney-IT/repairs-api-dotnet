using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Generated;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/repairs")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class RepairsController : Controller
    {
        private readonly IRaiseRepairUseCase _raiseRepairUseCase;
        private readonly IListWorkOrdersUseCase _listWorkOrdersUseCase;
        private readonly ICompleteWorkOrderUseCase _completeWorkOrderUseCase;
        private readonly IUpdateJobStatusUseCase _updateJobStatusUseCase;

        public RepairsController(
            IRaiseRepairUseCase raiseRepairUseCase,
            IListWorkOrdersUseCase listWorkOrdersUseCase,
            ICompleteWorkOrderUseCase completeWorkOrderUseCase,
            IUpdateJobStatusUseCase updateJobStatusUseCase
        )
        {
            _raiseRepairUseCase = raiseRepairUseCase;
            _listWorkOrdersUseCase = listWorkOrdersUseCase;
            _completeWorkOrderUseCase = completeWorkOrderUseCase;
            _updateJobStatusUseCase = updateJobStatusUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> RaiseRepair([FromBody] RaiseRepair request)
        {
            try
            {
                var result = await _raiseRepairUseCase.Execute(request.ToDb());
                return Ok(result);
            }
            catch (NotSupportedException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult GetList()
        {
            return Ok(_listWorkOrdersUseCase.Execute());
        }

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
        }

    }

}
