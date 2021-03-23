using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Factories;
using RepairsApi.V2.UseCase;
using RepairsApi.V2.UseCase.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RepairsApi.V2.Authorisation;
using Microsoft.AspNetCore.Authorization;

namespace RepairsApi.V2.Controllers
{
    [ApiController]
    [Route("/api/v2/properties")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class PropertiesController : BaseController
    {
        private readonly IListAlertsUseCase _listAlertsUseCase;
        private readonly IGetPropertyUseCase _getPropertyUseCase;
        private readonly IListPropertiesUseCase _listPropertiesUseCase;
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(
            IListAlertsUseCase listAlertsUseCase,
            IGetPropertyUseCase getPropertyUseCase,
            IListPropertiesUseCase listPropertiesUseCase,
            ILogger<PropertiesController> logger)
        {
            _listAlertsUseCase = listAlertsUseCase;
            _getPropertyUseCase = getPropertyUseCase;
            _listPropertiesUseCase = listPropertiesUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all matching properties given the query params
        /// </summary>
        /// <param name="address">A partial or full address</param>
        /// <param name="postcode">A postcode</param>
        /// <param name="q">A postcode or partial or full address</param>
        /// <response code="200">Properties found</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PropertyListItem>), 200)]
        [ProducesResponseType(typeof(string), StatusCodes.Status502BadGateway)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = UserGroups.AGENT)]
        public async Task<IActionResult> ListProperties([FromQuery] string address, [FromQuery] string postcode, [FromQuery] string q)
        {
            PropertySearchModel searchModel = new PropertySearchModel
            {
                Address = address,
                PostCode = postcode,
                Query = q
            };

            _logger.LogInformation("Listing properties");
            var properties = await _listPropertiesUseCase.ExecuteAsync(searchModel);

            List<PropertyListItem> response = properties.ToResponse();
            _logger.LogInformation($"Found {response.Count} properties");
            return Ok(response);
        }

        /// <summary>
        /// Retrieves a property
        /// </summary>
        /// <param name="propertyReference">The property reference</param>
        /// <response code="200">Property found</response>
        /// <response code="404">Property not found</response>
        [HttpGet]
        [Route("{propertyReference}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PropertyResponse), 200)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        [ProducesResponseType(typeof(string), StatusCodes.Status502BadGateway)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = UserGroups.AGENT + "," + UserGroups.CONTRACTOR + "," + UserGroups.CONTRACT_MANAGER)]
        public async Task<IActionResult> GetProperty([FromRoute][Required] string propertyReference)
        {
            var property = await _getPropertyUseCase.ExecuteAsync(propertyReference);

            return Ok(property.ToResponse());
        }

        /// <summary>
        /// Retrieves cautionary alerts
        /// </summary>
        /// <param name="propertyReference">The property reference</param>
        /// <response code="200">Gets all cautionary alerts for a property</response>
        [HttpGet]
        [Route("{propertyReference}/alerts")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CautionaryAlertResponseList), 200)]
        [ProducesResponseType(typeof(string), StatusCodes.Status502BadGateway)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = UserGroups.AGENT)]
        public async Task<IActionResult> ListCautionaryAlerts([FromRoute][Required] string propertyReference)
        {
            var alerts = await _listAlertsUseCase.ExecuteAsync(propertyReference);

            return Ok(alerts.ToResponse());
        }
    }
}
