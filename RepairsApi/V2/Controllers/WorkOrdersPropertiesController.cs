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
    public partial class WorkOrdersController : Controller
    {
        /// <summary>
        /// Returns a list of variations for work orders
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/variation-tasks")]
        [ProducesResponseType(typeof(IEnumerable<GetVariationResponse>), 200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = UserGroups.ContractManager)]
        public async Task<IActionResult> GetWorkOrderVariations(int id)
        {
            return Ok(await _listVariationTasksUseCase.Execute(id));
        }

        /// <summary>
        /// Gets a list of tasks for a given work order id
        /// </summary>
        /// <param name="id">work order id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/tasks")]
        [ProducesResponseType(typeof(IEnumerable<WorkOrderItemViewModel>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ListWorkOrderTasks(int id)
        {
            var result = await _listWorkOrderTasksUseCase.Execute(id);
            return Ok(result.ToResponse());
        }

        /// <summary>
        /// Gets a list of notes for a given work order id
        /// </summary>
        /// <param name="id">work order id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/notes")]
        [ProducesResponseType(typeof(IEnumerable<NoteListItem>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ListWorkOrderNotes(int id)
        {
            var result = await _listWorkOrderNotesUseCase.Execute(id);
            return Ok(result);
        }

    }

}
