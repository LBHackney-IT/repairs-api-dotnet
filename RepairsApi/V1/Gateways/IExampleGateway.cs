using System.Collections.Generic;
using RepairsApi.V1.Domain;

namespace RepairsApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
