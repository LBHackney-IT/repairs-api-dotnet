using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.UseCase;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/filter")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class FilterController : Controller
    {
        private readonly IGetFilterUseCase _getFilterUseCase;

        public FilterController(IGetFilterUseCase getFilterUseCase)
        {
            _getFilterUseCase = getFilterUseCase;
        }

        [HttpGet]
        [Route("{modelName}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ModelFilterConfiguration), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetFilterInformationAsync([Required] string modelName)
        {
            return Ok(await _getFilterUseCase.Execute(modelName));
        }
    }
}
