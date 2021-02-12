using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
#nullable enable
    public class ListSorTradesUseCase : IListSorTradesUseCase
    {
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public ListSorTradesUseCase(IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task<IEnumerable<SorTradeResponse>> Execute()
        {
            var sorCodes = await _scheduleOfRatesGateway.GetTrades();
            return sorCodes.Select(c => c.ToResponse());
        }
    }
}
