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
    [Route("/api/v2/repairs")]
    [Route("/api/v2/workOrders")]
    [ApiVersion("2.0")]
    public partial class WorkOrdersController : Controller
    {
        //private readonly IAuthorizationService _authorizationService;
        //private readonly ICreateWorkOrderUseCase _createWorkOrderUseCase;
        //private readonly IListWorkOrdersUseCase _listWorkOrdersUseCase;
        //private readonly ICompleteWorkOrderUseCase _completeWorkOrderUseCase;
        //private readonly IUpdateJobStatusUseCase _updateJobStatusUseCase;
        //private readonly IGetWorkOrderUseCase _getWorkOrderUseCase;
        //private readonly IListWorkOrderTasksUseCase _listWorkOrderTasksUseCase;
        //private readonly IListWorkOrderNotesUseCase _listWorkOrderNotesUseCase;
        //private readonly IFeatureManager _featureManager;

        //public WorkOrdersController(
        //    IAuthorizationService authorizationService,
        //    ICreateWorkOrderUseCase createWorkOrderUseCase,
        //    IListWorkOrdersUseCase listWorkOrdersUseCase,
        //    ICompleteWorkOrderUseCase completeWorkOrderUseCase,
        //    IUpdateJobStatusUseCase updateJobStatusUseCase,
        //    IGetWorkOrderUseCase getWorkOrderUseCase,
        //    IListWorkOrderTasksUseCase listWorkOrderTasksUseCase,
        //    IListWorkOrderNotesUseCase listWorkOrderNotesUseCase,
        //    IFeatureManager featureManager)
        //{
        //    _authorizationService = authorizationService;
        //    _createWorkOrderUseCase = createWorkOrderUseCase;
        //    _listWorkOrdersUseCase = listWorkOrdersUseCase;
        //    _completeWorkOrderUseCase = completeWorkOrderUseCase;
        //    _updateJobStatusUseCase = updateJobStatusUseCase;
        //    _getWorkOrderUseCase = getWorkOrderUseCase;
        //    _listWorkOrderTasksUseCase = listWorkOrderTasksUseCase;
        //    _listWorkOrderNotesUseCase = listWorkOrderNotesUseCase;
        //    _featureManager = featureManager;
        //}

        /// <summary>
        /// Returns a list of variations for work orders
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/variations")]
        //[ProducesResponseType(typeof(IEnumerable<WorkOrderItemViewModel>), 200)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        [Authorize(Roles = UserGroups.AGENT + "," + UserGroups.CONTRACT_MANAGER)]
        public IActionResult GetWorkOrderVariations(int id)
        {
            return Ok(id);
        }

    }

}
