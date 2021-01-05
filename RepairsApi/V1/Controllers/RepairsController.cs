using Microsoft.AspNetCore.Mvc;
using RepairsApi.V1.Domain.Repair;
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
        public async Task<IActionResult> RaiseRepair([FromBody] RaiseRepairRequest request)
        {
            var result = await _raiseRepairUseCase.Execute(request.ToDomain());

            if (result)
            {
                return Ok(request);
            }

            return BadRequest();
        }
    }

    public class RaiseRepairRequest
    {
        public string Description { get; set; }

        internal WorkOrder ToDomain()
        {
            WorkOrder raiseRepair = new WorkOrder();
            raiseRepair.DescriptionOfWork = Description;

            return raiseRepair;
        }
    }
}
