using JWT.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace RepairsApi.V2.Services
{
#nullable enable
    public class CurrentUserService : ICurrentUserLoader, ICurrentUserService
    {
        private readonly ILogger<CurrentUserService> _logger;
        private readonly GroupOptions _options;
        private ClaimsPrincipal? _user = null;

        public CurrentUserService(ILogger<CurrentUserService> logger, IOptions<GroupOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public void LoadUser(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt)) return;
            try
            {
                var jwtUser = new JwtBuilder()
                        .Decode<User>(jwt);

                _user = MapUser(jwtUser);
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

        private ClaimsPrincipal MapUser(User user)
        {
            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));

            var raiseLimit = 0.0;

            foreach (var group in user.Groups)
            {
                if (_options.SecurityGroups.TryGetValue(group, out PermissionsModel? perms))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, perms.SecurityGroup.ToString()));

                    if (!string.IsNullOrWhiteSpace(perms.ContractorReference))
                    {
                        identity.AddClaim(new Claim(CustomClaimTypes.CONTRACTOR, perms.ContractorReference));
                    }
                }

                if (Groups.RaiseLimitGroups.TryGetValue(group, out double limit))
                {
                    raiseLimit = Math.Max(raiseLimit, limit);
                }
            }

            identity.AddClaim(new Claim(CustomClaimTypes.RAISELIMIT, raiseLimit.ToString(CultureInfo.InvariantCulture)));

            return new ClaimsPrincipal(identity);
        }
    }
}
