using System;
using System.Collections.Generic;
using NUnit.Framework;
using RepairsApi.V1.Gateways;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using RepairsApi.V1.Infrastructure;

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
            // arrange
            var expected = CreateWorkOrder();

            // act
            await _classUnderTest.CreateWorkOrder(expected);
            await InMemoryDb.Instance.SaveChangesAsync();

            // assert
            InMemoryDb.Instance.WorkOrders.Should().ContainSingle().Which.IsSameOrEqualTo(expected);
        }

        private static WorkOrder CreateWorkOrder()
        {

            var expected = new WorkOrder
            {
                WorkPriority = new WorkPriority
                {
                    PriorityCode = RepairsApi.V1.Generated.WorkPriorityCode._1,
                    RequiredCompletionDateTime = DateTime.UtcNow
                },
                WorkClass = new WorkClass
                {
                    WorkClassCode = RepairsApi.V1.Generated.WorkClassCode._0
                },
                WorkElements = new List<WorkElement>(),
                DescriptionOfWork = "description"
            };
            return expected;
        }
    }
}
