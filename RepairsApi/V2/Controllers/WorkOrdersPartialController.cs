using Microsoft.AspNetCore.Mvc;
using RepairsApi.V2.Boundary;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Generated;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using RepairsApi.V2.Authorisation;
using Microsoft.FeatureManagement;

namespace RepairsApi.V2.Controllers
{
    //[Route("/api/v2/repairs")]
    //[Route("/api/v2/workOrders")]
    [ApiVersion("2.0")]
    public partial class WorkOrdersController : Controller
    {
        /// <summary>
        /// Returns a list of variations for work orders
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/variations")]
        [ProducesResponseType(typeof(IEnumerable<VariationTasksModel>), 200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = UserGroups.CONTRACT_MANAGER)]
        public async Task<IActionResult> GetWorkOrderVariations(int id)
        {
            return Ok(await _listVariationTasksUseCase.Execute(id));
        }

    }

}
