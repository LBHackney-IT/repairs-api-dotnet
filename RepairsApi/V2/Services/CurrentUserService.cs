using JWT.Builder;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RepairsApi.V2.Services
{
#nullable enable
    public class CurrentUserService : ICurrentUserLoader, ICurrentUserService
    {
        private readonly ILogger<CurrentUserService> _logger;
        private readonly IGroupsGateway _groupsGateway;
        private ClaimsPrincipal? _user = null;

        public CurrentUserService(ILogger<CurrentUserService> logger, IGroupsGateway groupsGateway)
        {
            _logger = logger;
            _groupsGateway = groupsGateway;
        }

        public async Task LoadUser(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt)) return;
            try
            {
                var jwtUser = new JwtBuilder()
                        .Decode<User>(jwt);

                _user = await MapUser(jwtUser);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public bool IsUserPresent()
        {
            return _user != null;
        }

        public ClaimsPrincipal? GetUser()
        {
            return _user;
        }

        public bool HasGroup(string groupName)
        {
            return _user?.HasClaim(ClaimTypes.Role, groupName) ?? false;
        }

        public bool TryGetContractor(out string contractor)
        {
            contractor = _user?.FindFirst(CustomClaimTypes.CONTRACTOR)?.Value ?? string.Empty;
            return !string.IsNullOrWhiteSpace(contractor);
        }

        private async Task<ClaimsPrincipal> MapUser(User user)
        {
            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));

            double varyLimit = 0;
            double raiseLimit = 0;

            foreach (var group in await _groupsGateway.GetMatchingGroups(user.Groups.ToArray()))
            {
                if (!string.IsNullOrWhiteSpace(group.ContractorReference))
                    identity.AddClaim(new Claim(CustomClaimTypes.CONTRACTOR, group.ContractorReference));

                if (!string.IsNullOrWhiteSpace(group.UserType))
                    identity.AddClaim(new Claim(ClaimTypes.Role, group.UserType));

                if (!string.IsNullOrWhiteSpace(group.UserType))
                    identity.AddClaim(new Claim(ClaimTypes.Role, group.UserType));

                if (group.VaryLimit.HasValue)
                    varyLimit = Math.Max(varyLimit, group.VaryLimit.Value);

                if (group.RaiseLimit.HasValue)
                    varyLimit = Math.Max(raiseLimit, group.RaiseLimit.Value);

            }

            identity.AddClaim(new Claim(CustomClaimTypes.RAISELIMIT, raiseLimit.ToString(CultureInfo.InvariantCulture)));
            identity.AddClaim(new Claim(CustomClaimTypes.VARYLIMIT, varyLimit.ToString(CultureInfo.InvariantCulture)));

            return new ClaimsPrincipal(identity);
        }
    }
}
