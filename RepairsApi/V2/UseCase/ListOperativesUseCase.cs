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
        private readonly IOperativesGateway _operativesGateway;
        private readonly IFilterBuilder<OperativeRequest, Operative> _filterBuilder;

        public ListOperativesUseCase(IOperativesGateway operativesGateway, IFilterBuilder<OperativeRequest, Operative> filterBuilder)
        {
            _operativesGateway = operativesGateway;
            _filterBuilder = filterBuilder;
        }

        public async Task<List<OperativeResponse>> ExecuteAsync(OperativeRequest searchModel)
        {
            var filter = _filterBuilder.BuildFilter(searchModel);
            var gatewayResponse = await _operativesGateway.ListByFilterAsync(filter);
            return gatewayResponse.Select(e => e.ToResponse()).ToList();
        }
    }
}
