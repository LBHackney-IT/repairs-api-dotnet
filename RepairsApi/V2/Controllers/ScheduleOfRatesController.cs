using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.UseCase.Interfaces;
using RepairsApi.V2.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Domain;
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
        private readonly IListSorTradesUseCase _listSorTrades;

        public ScheduleOfRatesController(
            IListScheduleOfRatesUseCase listScheduleOfRates,
            IListSorTradesUseCase listSorTrades
            )
        {
            _listScheduleOfRates = listScheduleOfRates;
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
        public async Task<IActionResult> ListRecords([FromQuery][Required] string tradeCode, [FromQuery][Required] string propertyReference)
        {
            return Ok(await _listScheduleOfRates.Execute(tradeCode, propertyReference));
        }

        /// <summary>
        /// Returns paged list of trades
        /// </summary>
        /// <response code="200">Success. Returns a list of SOR codes</response>
        [ProducesResponseType(typeof(IEnumerable<SorTradeResponse>), StatusCodes.Status200OK)]
        [Route("trades")]
        [HttpGet]
        public async Task<IActionResult> ListTrades()
        {
            return Ok(await _listSorTrades.Execute());
        }
    }
}
