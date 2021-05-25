using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Request;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class ListOperativesUseCase : IListOperativesUseCase
    {
        private readonly IOperativeGateway _operativeGateway;
        private readonly IFilterBuilder<OperativeRequest, Operative> _filterBuilder;

        public ListOperativesUseCase(IOperativeGateway operativeGateway, IFilterBuilder<OperativeRequest, Operative> filterBuilder)
        {
            _operativeGateway = operativeGateway;
            _filterBuilder = filterBuilder;
        }

        public async Task<List<OperativeResponse>> ExecuteAsync(OperativeRequest searchModel)
        {
            var filter = _filterBuilder.BuildFilter(searchModel);
            var gatewayResponse = await _operativeGateway.ListByFilterAsync(filter);
            return gatewayResponse.Select(e => e.ToResponse()).ToList();
        }
    }
}
