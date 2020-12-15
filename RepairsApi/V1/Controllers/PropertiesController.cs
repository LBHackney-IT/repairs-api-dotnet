using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RepairsApi.V1.Boundary.Response;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V1.Controllers
{
    [ApiController]
    [Route("/api/v1/properties")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PropertiesController : BaseController
    { 
        /// <summary>
        /// Retrieves all matching properties given the query params
        /// </summary>
        /// <param name="address">A partial or full address</param>
        /// <param name="postcode">A postcode</param>
        /// <param name="q">A postcode or partial or full address</param>
        /// <response code="200">Properties found</response>
        /// <response code="401">Invalid auth token</response>
        [HttpGet]
        public virtual IActionResult ListProperties([FromQuery]string address, [FromQuery]string postcode, [FromQuery]string q)
        { 
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<InlineResponse2002>));

            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401, default(InlineResponse401));
            string exampleJson = null;
            exampleJson = "[ {\n  \"address\" : {\n    \"shortAddress\" : \"shortAddress\",\n    \"postalCode\" : \"postalCode\",\n    \"streetSuffix\" : \"streetSuffix\",\n    \"addressLine\" : \"addressLine\"\n  },\n  \"hierarchyType\" : {\n    \"levelCode\" : \"levelCode\",\n    \"subTypeCode\" : \"subTypeCode\",\n    \"subTypeDescription\" : \"subTypeDescription\"\n  },\n  \"propertyReference\" : \"propertyReference\"\n}, {\n  \"address\" : {\n    \"shortAddress\" : \"shortAddress\",\n    \"postalCode\" : \"postalCode\",\n    \"streetSuffix\" : \"streetSuffix\",\n    \"addressLine\" : \"addressLine\"\n  },\n  \"hierarchyType\" : {\n    \"levelCode\" : \"levelCode\",\n    \"subTypeCode\" : \"subTypeCode\",\n    \"subTypeDescription\" : \"subTypeDescription\"\n  },\n  \"propertyReference\" : \"propertyReference\"\n} ]";
            
                        var example = exampleJson != null
                        ? JsonConvert.DeserializeObject<List<PropertyViewModel>>(exampleJson)
                        : default(List<PropertyViewModel>);            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Retrieves a property
        /// </summary>
        /// <param name="propertyReference">The property reference</param>
        /// <response code="200">Property found</response>
        /// <response code="401">Invalid auth token</response>
        /// <response code="404">Property not found</response>
        [HttpGet]
        [Route("{propertyReference}")]
        public virtual IActionResult GetProperty([FromRoute][Required]string propertyReference)
        { 
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(InlineResponse2001));

            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401, default(InlineResponse401));

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404, default(InlineResponse404));
            string exampleJson = null;
            exampleJson = "{\n  \"property\" : {\n    \"address\" : {\n      \"shortAddress\" : \"shortAddress\",\n      \"postalCode\" : \"postalCode\",\n      \"streetSuffix\" : \"streetSuffix\",\n      \"addressLine\" : \"addressLine\"\n    },\n    \"hierarchyType\" : {\n      \"levelCode\" : \"levelCode\",\n      \"subTypeCode\" : \"subTypeCode\",\n      \"subTypeDescription\" : \"subTypeDescription\"\n    },\n    \"propertyReference\" : \"propertyReference\"\n  },\n  \"cautionaryAlerts\" : [ {\n    \"alertCode\" : \"alertCode\",\n    \"endDate\" : \"endDate\",\n    \"description\" : \"description\",\n    \"startDate\" : \"startDate\"\n  }, {\n    \"alertCode\" : \"alertCode\",\n    \"endDate\" : \"endDate\",\n    \"description\" : \"description\",\n    \"startDate\" : \"startDate\"\n  } ]\n}";
            
                        var example = exampleJson != null
                        ? JsonConvert.DeserializeObject<PropertyResponse>(exampleJson)
                        : default(PropertyResponse);            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Retrieves cautionary alerts
        /// </summary>
        /// <param name="propertyReference">The property reference</param>
        /// <response code="200">Gets all cautionary alerts for a property</response>
        /// <response code="401">Invalid auth token</response>
        /// <response code="404">Property not found</response>
        [HttpGet]
        [Route("{propertyReference}/alerts")]
        public virtual IActionResult ListCautionaryAlerts([FromRoute][Required] string propertyReference)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(InlineResponse200));

            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401, default(InlineResponse401));

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404, default(InlineResponse404));
            string exampleJson = null;
            exampleJson = "{\n  \"alerts\" : [ {\n    \"alertCode\" : \"alertCode\",\n    \"endDate\" : \"endDate\",\n    \"description\" : \"description\",\n    \"startDate\" : \"startDate\"\n  }, {\n    \"alertCode\" : \"alertCode\",\n    \"endDate\" : \"endDate\",\n    \"description\" : \"description\",\n    \"startDate\" : \"startDate\"\n  } ],\n  \"propertyReference\" : \"propertyReference\"\n}";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<CautionaryAlertResponseList>(exampleJson)
            : default(CautionaryAlertResponseList);            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
