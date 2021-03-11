using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Services;
using RepairsApi.V2.Boundary.Response;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/hub-user")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class HubUserController : BaseController
    {
        private readonly ICurrentUserService _currentUserService;

        public HubUserController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Gets repairs hub user
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [ProducesResponseType(typeof(HubUserModel), 200)]
        [HttpGet]
        [Authorize(Roles = UserGroups.AGENT + "," + UserGroups.CONTRACTOR)]
        public async Task<ActionResult> GetHubUser()
        {
            return Ok(await Task.FromResult(_currentUserService.GetHubUser()));
        }
    }
}
