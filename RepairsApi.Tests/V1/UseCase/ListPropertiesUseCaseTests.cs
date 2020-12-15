using Moq;
using NUnit.Framework;
using RepairsApi.V1.Gateways;

namespace RepairsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class ListPropertiesUseCaseTests
    {
        private Mock<IPropertyGateway> _gatewayMock;

        [SetUp]
        public void Setup()
        {
            _gatewayMock = new Mock<IPropertyGateway>();
        }
    }
}
