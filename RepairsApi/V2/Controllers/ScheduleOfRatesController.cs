using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.UseCase.Interfaces;
using RepairsApi.V2.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Domain;

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
        private readonly IListSorContractorsUseCase _listSorContractors;

        public ScheduleOfRatesController(
            IListScheduleOfRatesUseCase listScheduleOfRates,
            IListSorTradesUseCase listSorTrades,
            IListSorContractorsUseCase listSorContractors
            )
        {
            _listScheduleOfRates = listScheduleOfRates;
            _listSorTrades = listSorTrades;
            _listSorContractors = listSorContractors;
        }

        /// <summary>
        /// Returns paged list of SOR codes
        /// </summary>
        /// <response code="200">Success. Returns a list of SOR codes</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(IEnumerable<ScheduleOfRatesModel>), StatusCodes.Status200OK)]
        [Route("codes")]
        [HttpGet]
        public async Task<IActionResult> ListRecords([FromQuery] string tradeCode, [FromQuery] string contractorReference)
        {
            return Ok(await _listScheduleOfRates.Execute(tradeCode, contractorReference));
        }

        /// <summary>
        /// Returns paged list of trades
        /// </summary>
        /// <response code="200">Success. Returns a list of SOR codes</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(IEnumerable<SorTrade>), StatusCodes.Status200OK)]
        [Route("trades")]
        [HttpGet]
        public async Task<IActionResult> ListTrades()
        {
            return Ok(await _listSorTrades.Execute());
        }

        /// <summary>
        /// Returns List of contractors
        /// </summary>
        /// <response code="200">Success. Returns a list of SOR codes</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(IEnumerable<Contractor>), StatusCodes.Status200OK)]
        [Route("contractors")]
        [HttpGet]
        public async Task<IActionResult> ListContractors()
        {
            return Ok(await _listSorContractors.Execute());
        }
    }
}
