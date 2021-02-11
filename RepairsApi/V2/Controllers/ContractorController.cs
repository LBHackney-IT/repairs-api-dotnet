using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Gateways;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/contractors")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class ContractorController : BaseController
    {
        private readonly IScheduleOfRatesGateway _sorGateway;

        public ContractorController(IScheduleOfRatesGateway sorGateway)
        {
            _sorGateway = sorGateway;
        }

        [HttpGet]
        public async Task<IActionResult> ListContractors([FromQuery] string propertyReference, [FromQuery] string tradeCode)
        {
            var contractors = await _sorGateway.GetContractors(propertyReference, tradeCode);

            return Ok(contractors);
        }
    }

}
