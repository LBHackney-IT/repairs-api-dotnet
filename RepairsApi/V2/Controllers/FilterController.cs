using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/filter")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class FilterController : Controller
    {
        private readonly FilterConfiguration _options;

        public FilterController(IOptions<FilterConfiguration> options)
        {
            _options = options.Value;
        }

        [HttpGet]
        [Route("{modelName}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Dictionary<string, List<FilterOption>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public IActionResult GetFilterInformation([Required]string modelName)
        {
            if (_options.TryGetValue(modelName, out var config))
            {
                return Ok(config);
            }
            else
            {
                return NotFound($"No filter configuration set up for {modelName}");
            }
        }
    }

    public class FilterConfiguration : Dictionary<string, Dictionary<string, List<FilterOption>>>
    {

    }

    public class FilterOption
    {
        public string Key { get; set; }
        public string Description { get; set; }
    }
}
