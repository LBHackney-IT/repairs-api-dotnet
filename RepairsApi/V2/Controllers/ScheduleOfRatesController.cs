using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.UseCase.Interfaces;
using RepairsApi.V2.Boundary.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;

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

        public ScheduleOfRatesController(IListScheduleOfRatesUseCase listScheduleOfRates, ISorPriorityGateway priorityGateway)
        {
            _listScheduleOfRates = listScheduleOfRates;
            _priorityGateway = priorityGateway;
        }

        /// <summary>
        /// Returns paged list of SOR codes
        /// </summary>
        /// <response code="200">Success. Returns a list of SOR codes</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(IEnumerable<ScheduleOfRatesModel>), StatusCodes.Status200OK)]
        [Route("codes")]
        [HttpGet]
        public async Task<IActionResult> ListRecords()
        {
            return Ok(await _listScheduleOfRates.Execute());
            //var sorPattern = "^[A-Za-z0-9]{7,8}$";
        }
        [HttpGet]
        [Route("/codes/{sorCode}")]
        public IActionResult ViewRecord(string sorCode)
        {
            return Ok(new { sor = sorCode });
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
