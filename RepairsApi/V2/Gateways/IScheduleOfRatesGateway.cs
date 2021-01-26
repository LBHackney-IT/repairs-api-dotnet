using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public interface IScheduleOfRatesGateway
    {
        Task<IEnumerable<ScheduleOfRates>> GetSorCodes(string contractorRef, string tradeCode);
        Task<IEnumerable<SORTrade>> GetTrades();
        Task<IEnumerable<SORContractor>> GetContractors();
    }

}
