using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Domain;

namespace RepairsApi.V2.Gateways
{
    public interface IResidentContactGateway
    {
        Task<IEnumerable<ResidentContact>> GetByHouseholdReferenceAsync(string householdReference);
    }
}
