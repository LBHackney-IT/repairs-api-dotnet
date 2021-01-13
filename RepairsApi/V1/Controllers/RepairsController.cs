using Microsoft.AspNetCore.Mvc;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Generated;
using RepairsApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V1.Controllers
{
    [ApiController]
    [Route("/api/v2/repairs")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class RepairsController : Controller
    {
        private readonly IRaiseRepairUseCase _raiseRepairUseCase;
        private readonly IListWorkOrdersUseCase _listWorkOrdersUseCase;

        public RepairsController(
            IRaiseRepairUseCase raiseRepairUseCase,
            IListWorkOrdersUseCase listWorkOrdersUseCase)
        {
            _raiseRepairUseCase = raiseRepairUseCase;
            _listWorkOrdersUseCase = listWorkOrdersUseCase;
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
    }

}
