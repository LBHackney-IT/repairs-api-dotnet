using NUnit.Framework;
using RepairsApi.V2.Gateways;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Gateways
{
    public class AppointmentGatewayTests
    {
        private AppointmentGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new AppointmentGateway(InMemoryDb.Instance);
        }

        [Test]
        public async Task Lists()
        {
            
        }
    }
}
