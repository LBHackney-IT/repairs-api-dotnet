using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/hub-user")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class HubUserController : Controller
    {
        private readonly ICurrentUserService _currentUserService;

        private HubUserController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Gets repairs hub user
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet]
        [Authorize(Roles = UserGroups.AGENT + "," + UserGroups.CONTRACTOR)]
        public IActionResult GetHubUser()
        {
            
            return Ok();
        }
    }
}
