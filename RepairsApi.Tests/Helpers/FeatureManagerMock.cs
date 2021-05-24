using Microsoft.FeatureManagement;
using Moq;

namespace RepairsApi.Tests.Helpers
{
    public class FeatureManagerMock : Mock<IFeatureManager>
    {
        public void SetFeature(string featureName, bool isEnabled)
        {
            Setup(x => x.IsEnabledAsync(featureName))
                .ReturnsAsync(isEnabled);
        }
    }
}
