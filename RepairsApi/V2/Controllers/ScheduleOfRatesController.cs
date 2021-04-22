using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure.Hackney;
using System.ComponentModel.DataAnnotations;
using RepairsApi.V2.Authorisation;
using Microsoft.AspNetCore.Authorization;

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
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;
        private readonly IListSorTradesUseCase _listSorTrades;

        public ScheduleOfRatesController(
            IListScheduleOfRatesUseCase listScheduleOfRates,
            IListSorTradesUseCase listSorTrades,
            ISorPriorityGateway priorityGateway,
            IScheduleOfRatesGateway scheduleOfRatesGateway
            )
        {
            _listScheduleOfRates = listScheduleOfRates;
            _priorityGateway = priorityGateway;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = UserGroups.Agent + "," + UserGroups.ContractManager)]
        public async Task<IActionResult> ListRecords([FromQuery][Required] string tradeCode, [FromQuery][Required] string propertyReference, [FromQuery][Required] string contractorReference)
        {
            return Ok(await _listScheduleOfRates.Execute(tradeCode, propertyReference, contractorReference));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sorCode"></param>
        /// <param name="propertyReference"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<ScheduleOfRatesModel>), StatusCodes.Status200OK)]
        [Route("codes/{sorCode}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = UserGroups.Contractor + "," + UserGroups.ContractManager)]
        public async Task<IActionResult> GetSorCode([Required] string sorCode, [FromQuery][Required] string propertyReference)
        {
            var contractorReference = User.FindFirst(CustomClaimTypes.Contractor).Value;

            return Ok(await _scheduleOfRatesGateway.GetCode(sorCode, propertyReference, contractorReference));
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
        [Authorize(Roles = UserGroups.Agent + "," + UserGroups.ContractManager)]
        public async Task<IActionResult> ListPriorities()
        {
            return Ok(await _priorityGateway.GetPriorities());
        }
    }
}
