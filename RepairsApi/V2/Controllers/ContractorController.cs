using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Domain;
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

        /// <summary>
        /// Gets valid contractors for a valid trade and property
        /// </summary>
        /// <param name="propertyReference"></param>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<Contractor>), 200)]
        [HttpGet]
        public async Task<IActionResult> ListContractors([FromQuery][Required] string propertyReference, [FromQuery][Required] string tradeCode)
        {
            var contractors = await _sorGateway.GetContractors(propertyReference, tradeCode);

            return Ok(contractors);
        }
    }

}
