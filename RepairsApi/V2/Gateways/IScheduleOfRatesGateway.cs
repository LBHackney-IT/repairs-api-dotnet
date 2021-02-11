using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using Contractor = RepairsApi.V2.Domain.Contractor;

namespace RepairsApi.V2.Gateways
{
    public interface IScheduleOfRatesGateway
    {
        Task<IEnumerable<SorCodeTrade>> GetTrades();
        Task<IEnumerable<SorCodeResult>> GetSorCodes(string propertyReference, string tradeCode);
        Task<double?> GetCost(string contractReference, string sorCode);
        Task<IEnumerable<string>> GetContracts(string contractorReference);
        [Obsolete("Use property reference to filter list")]
        Task<IEnumerable<LegacyScheduleOfRatesModel>> GetSorCodes();
        Task<IEnumerable<Contractor>> GetContractors(string propertyRef, string tradeCode);
    }

}
