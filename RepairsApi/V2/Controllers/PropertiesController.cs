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
        [ProducesResponseType(typeof(List<PropertyViewModel>), 200)]
        public async Task<IActionResult> ListProperties([FromQuery] string address, [FromQuery] string postcode, [FromQuery] string q)
        {
            IEnumerable<PropertyModel> properties;

            PropertySearchModel searchModel = new PropertySearchModel
            {
                Address = address,
                PostCode = postcode,
                Query = q
            };

            try
            {
                _logger.LogInformation("Listing properties");
                properties = await _listPropertiesUseCase.ExecuteAsync(searchModel);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode);
            }

            List<PropertyViewModel> response = properties.ToResponse();
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
        public async Task<IActionResult> GetProperty([FromRoute][Required] string propertyReference)
        {
            PropertyWithAlerts property;

            try
            {
                property = await _getPropertyUseCase.ExecuteAsync(propertyReference);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode);
            }

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
        public async Task<IActionResult> ListCautionaryAlerts([FromRoute][Required] string propertyReference)
        {
            AlertList alerts;

            try
            {
                alerts = await _listAlertsUseCase.ExecuteAsync(propertyReference);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode);
            }

            return Ok(alerts.ToResponse());
        }
    }
}
