using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure.Hackney;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("api/v2/schedule-of-rates")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class ScheduleOfRatesController : Controller
    {

        private readonly IListScheduleOfRatesUseCase _listScheduleOfRates;
        private readonly ISorPriorityGateway _priorityGateway;
        private readonly IListSorTradesUseCase _listSorTrades;

        public ScheduleOfRatesController(
            IListScheduleOfRatesUseCase listScheduleOfRates,
            IListSorTradesUseCase listSorTrades,
            ISorPriorityGateway priorityGateway
            )
        {
            _listScheduleOfRates = listScheduleOfRates;
            _priorityGateway = priorityGateway;
            _listSorTrades = listSorTrades;
        }

        /// <summary>
        /// Returns paged list of SOR codes
        /// </summary>
        /// <response code="200">Success. Returns a list of SOR codes</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(IEnumerable<ScheduleOfRatesModel>), StatusCodes.Status200OK)]
        [Route("codes")]
        [HttpGet]
        public async Task<IActionResult> ListRecords([FromQuery] string tradeCode, [FromQuery] string propertyReference, [FromQuery] string contractorReference)
        {
            if (string.IsNullOrWhiteSpace(tradeCode) && string.IsNullOrWhiteSpace(propertyReference) && string.IsNullOrWhiteSpace(contractorReference))
                return Ok(await _listScheduleOfRates.Execute());
            else if (!string.IsNullOrWhiteSpace(tradeCode) && !string.IsNullOrWhiteSpace(propertyReference) && !string.IsNullOrWhiteSpace(contractorReference))
                return Ok(await _listScheduleOfRates.Execute(tradeCode, propertyReference, contractorReference));

            return BadRequest();
        }

        /// <summary>
        /// Returns paged list of trades
        /// </summary>
        /// <response code="200">Success. Returns a list of SOR codes</response>
        [ProducesResponseType(typeof(IEnumerable<SorTradeResponse>), StatusCodes.Status200OK)]
        [Route("trades")]
        [HttpGet]
        public async Task<IActionResult> ListTrades([FromQuery][Required] string propRef)
        {
            return Ok(await _listSorTrades.Execute(propRef));
        }


        /// <summary>
        /// Returns list of SOR Code Priorities
        /// </summary>
        [ProducesResponseType(typeof(IEnumerable<SORPriority>), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("priorities")]
        public async Task<IActionResult> ListPriorities()
        {
            return Ok(await _priorityGateway.GetPriorities());
        }
    }
}
