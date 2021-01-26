using RepairsApi.V2.Controllers;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
#nullable enable
    public class ListSorContractorsUseCase : IListSorContractorsUseCase
    {
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public ListSorContractorsUseCase(IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task<IEnumerable<Contractor>> Execute()
        {
            var contractors = await _scheduleOfRatesGateway.GetContractors();
            return contractors.Select(c => c.ToResponse());
        }
    }
}
