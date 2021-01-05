using NUnit.Framework;
using RepairsApi.V1.Domain.Repair;
using RepairsApi.V1.Gateways;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V1.Gateways
{
    public class RepairGatewayTests
    {
        private RepairsGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new RepairsGateway(InMemoryDb.Instance);
        }

        [Test]
        public async Task Run()
        {
            await _classUnderTest.CreateWorkOrder(new WorkOrder());
        }
    }
}
