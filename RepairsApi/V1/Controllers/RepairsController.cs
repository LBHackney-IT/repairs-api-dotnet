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

        public RepairsController(IRaiseRepairUseCase raiseRepairUseCase)
        {
            _raiseRepairUseCase = raiseRepairUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> RaiseRepair([FromBody] RaiseRepair request)
        {
            try
            {
                var result = await _raiseRepairUseCase.Execute(request.ToDomain());
                return Ok(result);
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return StatusCode(500);
        }
    }

}
