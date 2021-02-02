using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;

namespace RepairsApi.V2.Gateways
{
    public interface IScheduleOfRatesGateway
    {
        Task<IEnumerable<SorCodeTrade>> GetTrades();
        Task<IEnumerable<SorCodeResult>> GetSorCodes(string propertyReference, string tradeCode);
        Task<double?> GetCost(string contractReference, string sorCode);
        Task<IEnumerable<string>> GetContracts(string contractorReference);
    }

}
