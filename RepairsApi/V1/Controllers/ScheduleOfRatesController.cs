using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V1.Controllers
{
    [ApiController]
    [Route("/api/v2/schedule-of-rates")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class ScheduleOfRatesController : Controller
    {

        public ScheduleOfRatesController()
        {

        }

        /// <summary>
        /// Returns paged list of SOR codes
        /// </summary>
        /// <response code="200">Success. Returns a list of SOR codes</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [Route("/codes")]
        [HttpGet]
        public IActionResult ListRecords()
        {
            return Ok(new { sor = "sor" });
        }

        [HttpGet]
        [Route("/codes/{sorCode}")]
        public IActionResult ViewRecord(string sorCode)
        {
            return Ok(new { sor = sorCode });
        }
        //public IActionResult Index()
        //{
        //    //var sorPattern = "^[A-Za-z0-9]{7,8}$";
        //    return View();
        //}
    }
}
