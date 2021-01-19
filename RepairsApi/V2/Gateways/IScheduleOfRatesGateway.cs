using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public interface IScheduleOfRatesGateway
    {
        public Task<IEnumerable<ScheduleOfRates>> GetSorCodes(string contractorRef = null);
        Task<string> GetContractorReference(string customCode);
    }

}
