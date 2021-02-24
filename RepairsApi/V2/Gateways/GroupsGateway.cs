using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
    public interface IGroupsGateway
    {
        Task<IEnumerable<SecurityGroup>> GetMatchingGroups(params string[] groups);
    }

    public class GroupsGateway : IGroupsGateway
    {
        private readonly RepairsContext _repairsContext;

        public GroupsGateway(RepairsContext repairsContext)
        {
            _repairsContext = repairsContext;
        }

        public async Task<IEnumerable<SecurityGroup>> GetMatchingGroups(params string[] groups)
        {
            return await _repairsContext.SecurityGroups.Where(sg => groups.Contains(sg.GroupName)).ToListAsync();
        }
    }
}
