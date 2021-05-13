using System.Collections.Generic;
using Moq;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Services;
using System.Linq;
using System.Security.Claims;

namespace RepairsApi.Tests.Helpers
{
    public class CurrentUserServiceMock : Mock<ICurrentUserService>
    {
        public void SetSecurityGroup(string group, bool isInGroup = true)
        {
            Setup(m => m.HasGroup(group)).Returns(isInGroup);
            Setup(c => c.HasAnyGroup(It.Is<string[]>(s => s.Contains(group)))).Returns(isInGroup);
        }

        public void SetContractor(params string[] contractors)
        {
            Setup(m => m.GetContractors()).Returns(contractors.ToList());
        }

        public void SetUser(string id, string email, string name, string varyLimit = null, string raiseLimit = null, List<string> contractors = null)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Email, email));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, id));
            identity.AddClaim(new Claim(ClaimTypes.Name, name));
            identity.AddClaims(contractors.Select(c => new Claim(CustomClaimTypes.Contractor, c)));
            if (!(varyLimit is null)) identity.AddClaim(new Claim(CustomClaimTypes.VaryLimit, varyLimit));
            if (!(raiseLimit is null)) identity.AddClaim(new Claim(CustomClaimTypes.RaiseLimit, raiseLimit));

            ClaimsPrincipal user = new ClaimsPrincipal(identity);
            Setup(x => x.GetUser())
                .Returns(user);
            Setup(x => x.IsUserPresent())
                .Returns(true);
        }
    }
}
