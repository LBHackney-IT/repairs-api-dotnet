using RepairsApi.V2.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
#nullable enable
    public interface IListScheduleOfRatesUseCase
    {
        Task<IEnumerable<ScheduleOfRatesModel>> Execute(string tradeCode, string contractorReference);
        [Obsolete("Use overload specifying trade and property")]
        Task<IEnumerable<LegacyScheduleOfRatesModel>> Execute();
    }
}
