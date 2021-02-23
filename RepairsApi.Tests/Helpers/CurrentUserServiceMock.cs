using Moq;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Services;

namespace RepairsApi.Tests.Helpers
{
    public class CurrentUserServiceMock : Mock<ICurrentUserService>
    {
        public void SetSecurityGroup(string group, bool isInGroup)
        {
            Setup(m => m.HasGroup(group)).Returns(isInGroup);
        }

        public void SetContractor(string contractor)
        {
            Setup(m => m.TryGetContractor(out contractor)).Returns(true);
        }
    }
}
