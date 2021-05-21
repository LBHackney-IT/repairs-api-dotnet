using Microsoft.AspNetCore.Authorization;
using Moq;
using System.Security.Claims;

namespace RepairsApi.Tests.Helpers
{
    public class AuthorisationMock : Mock<IAuthorizationService>
    {
        public void SetPolicyResult(string policy, bool result)
        {
            var policyResult = result ? AuthorizationResult.Success() : AuthorizationResult.Failed();
            Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), policy))
                .ReturnsAsync(policyResult);
        }
    }
}
