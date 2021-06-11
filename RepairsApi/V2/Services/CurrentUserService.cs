using JWT.Builder;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RepairsApi.V2.Services
{
#nullable enable
    public class CurrentUserService : ICurrentUserLoader, ICurrentUserService
    {
        private readonly ILogger<CurrentUserService> _logger;
        private readonly IGroupsGateway _groupsGateway;
        private readonly IHttpContextAccessor _context;
        private ClaimsPrincipal? User
        {
            get => _context.HttpContext.User;
            set => _context.HttpContext.User = value;
        }

        public CurrentUserService(ILogger<CurrentUserService> logger, IGroupsGateway groupsGateway, IHttpContextAccessor context)
        {
            _logger = logger;
            _groupsGateway = groupsGateway;
            _context = context;
        }

        public async Task LoadUser(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt)) return;

            try
            {
                var jwtUser = new JwtBuilder()
                    .Decode<User>(jwt);

                _logger.LogInformation("User Loaded with {email}", jwtUser.Email);

                User = await MapUser(jwtUser);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public bool IsUserPresent()
        {
            return User != null;
        }

        public ClaimsPrincipal? GetUser()
        {
            return User;
        }

        public bool HasGroup(string groupName)
        {
            return User?.HasClaim(ClaimTypes.Role, groupName) ?? false;
        }

        public List<string> GetContractors()
        {
            return User?.Contractors() ?? new List<string>();
        }

        private async Task<ClaimsPrincipal> MapUser(User user)
        {
            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, user.Sub));

            double varyLimit = 0;
            double raiseLimit = 0;

            foreach (var group in await _groupsGateway.GetMatchingGroups(user.Groups.ToArray()))
            {
                if (!string.IsNullOrWhiteSpace(group.ContractorReference))
                    identity.AddClaim(new Claim(CustomClaimTypes.Contractor, group.ContractorReference));

                if (!string.IsNullOrWhiteSpace(group.UserType))
                    identity.AddClaim(new Claim(ClaimTypes.Role, group.UserType));

                if (group.VaryLimit.HasValue)
                    varyLimit = Math.Max(varyLimit, group.VaryLimit.Value);

                if (group.RaiseLimit.HasValue)
                    raiseLimit = Math.Max(raiseLimit, group.RaiseLimit.Value);

            }

            identity.AddClaim(new Claim(CustomClaimTypes.RaiseLimit, raiseLimit.ToString(CultureInfo.InvariantCulture)));
            identity.AddClaim(new Claim(CustomClaimTypes.VaryLimit, varyLimit.ToString(CultureInfo.InvariantCulture)));

            return new ClaimsPrincipal(identity);
        }

        public bool HasAnyGroup(params string[] groups)
        {
            return groups.Any(g => HasGroup(g));
        }
    }
}
