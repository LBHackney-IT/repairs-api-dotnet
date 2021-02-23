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
        Task<IEnumerable<SorCodeTrade>> GetTrades(string propRef);
        Task<IEnumerable<ScheduleOfRatesModel>> GetSorCodes(string propertyReference, string tradeCode, string contractorReference);
        Task<double?> GetCost(string contractReference, string sorCode);
        Task<IEnumerable<string>> GetContracts(string contractorReference);
        Task<IEnumerable<Contractor>> GetContractors(string propertyRef, string tradeCode);
        Task<ScheduleOfRatesModel> GetCode(string sorCode, string propertyReference, string contractorReference);
    }

}
