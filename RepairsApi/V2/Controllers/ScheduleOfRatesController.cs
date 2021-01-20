using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.UseCase.Interfaces;
using RepairsApi.V2.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("api/v2/schedule-of-rates")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class ScheduleOfRatesController : Controller
    {

        private IListScheduleOfRatesUseCase _listScheduleOfRates;
        public ScheduleOfRatesController(IListScheduleOfRatesUseCase listScheduleOfRates)
        {
            _listScheduleOfRates = listScheduleOfRates;
        }

        /// <summary>
        /// Returns paged list of SOR codes
        /// </summary>
        /// <response code="200">Success. Returns a list of SOR codes</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
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
    }
}
