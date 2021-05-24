using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Boundary.Request;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/operatives")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class OperativesController : BaseController
    {
        private readonly IListOperativesUseCase _listOperativesUseCase;
        private readonly IGetOperativeUseCase _getOperativeUseCase;

        public OperativesController(IListOperativesUseCase listOperativesUseCase, IGetOperativeUseCase getOperativeUseCase)
        {
            _listOperativesUseCase = listOperativesUseCase;
            _getOperativeUseCase = getOperativeUseCase;
        }

        /// <summary>
        /// Retrieves the operative with the given payroll number
        /// </summary>
        /// <param name="operativePayrollNumber">The payroll number to match against</param>
        /// <response code="200">Operative found</response>
        [HttpGet]
        [Route("{operativePayrollNumber}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OperativeRequest), 200)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = UserGroups.Agent + "," + UserGroups.ContractManager + "," + UserGroups.AuthorisationManager)]
        public async Task<IActionResult> GetOperative([FromRoute] [Required] string operativePayrollNumber)
        {
            var result = await _getOperativeUseCase.ExecuteAsync(operativePayrollNumber);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all matching operatives given the query params
        /// </summary>
        /// <param name="operativeRequest">The search parameters object to match against</param>
        /// <response code="200">Operatives found</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<OperativeRequest>), 200)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = UserGroups.Agent + "," + UserGroups.ContractManager + "," + UserGroups.AuthorisationManager)]
        public async Task<IActionResult> ListOperatives(OperativeRequest operativeRequest)
        {
            var result = await _listOperativesUseCase.ExecuteAsync(operativeRequest);
            return Ok(result);
        }
    }
}
