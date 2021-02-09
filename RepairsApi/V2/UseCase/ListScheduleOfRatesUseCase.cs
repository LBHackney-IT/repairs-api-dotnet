using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
#nullable enable
    public class ListScheduleOfRatesUseCase : IListScheduleOfRatesUseCase
    {
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public ListScheduleOfRatesUseCase(IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task<IEnumerable<ScheduleOfRatesModel>> Execute(string tradeCode, string propertyReference)
        {
            var sorCodes = await _scheduleOfRatesGateway.GetSorCodes(propertyReference, tradeCode);
            return sorCodes.Select(c => c.ToResponse());
        }

        [Obsolete("Use overload specifying trade and property")]
        public async Task<IEnumerable<LegacyScheduleOfRatesModel>> Execute()
        {
            return await _scheduleOfRatesGateway.GetSorCodes();
        }
    }
}
