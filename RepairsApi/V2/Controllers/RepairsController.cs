using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Generated;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

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

        public RepairsController(
            IRaiseRepairUseCase raiseRepairUseCase,
            IListWorkOrdersUseCase listWorkOrdersUseCase,
            ICompleteWorkOrderUseCase completeWorkOrderUseCase
        )
        {
            _raiseRepairUseCase = raiseRepairUseCase;
            _listWorkOrdersUseCase = listWorkOrdersUseCase;
            _completeWorkOrderUseCase = completeWorkOrderUseCase;
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
            var result = await _completeWorkOrderUseCase.Execute(request);

            return result ? (IActionResult) Ok() : BadRequest();
        }
    }

}
