using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public interface IScheduleOfRatesGateway
    {
        Task<string> GetContractorReference(string customCode);
        Task<double> GetCost(string customCode);
        Task<IEnumerable<ScheduleOfRates>> GetSorCodes(string contractorRef, string tradeCode);
        Task<IEnumerable<SORTrade>> GetTrades();
        Task<IEnumerable<SORContractor>> GetContractors(string propRef);
        Task<IEnumerable<ScheduleOfRates>> GetSorCodes(string contractorRef);
    }

}
