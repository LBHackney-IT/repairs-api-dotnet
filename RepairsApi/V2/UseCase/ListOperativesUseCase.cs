using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class ListOperativesUseCase : IListOperativesUseCase
    {
        private readonly IOperativeGateway _operativeGateway;

        public ListOperativesUseCase(IOperativeGateway operativeGateway)
        {
            _operativeGateway = operativeGateway;
        }

        public async Task<List<Operative>> ExecuteAsync(Boundary.Request.Operative searchModel)
        {
            var gatewayResponse = await _operativeGateway.GetByQueryAsync(searchModel);
            return gatewayResponse.Select(e => e.ToResponse()).ToList();
        }
    }
}
