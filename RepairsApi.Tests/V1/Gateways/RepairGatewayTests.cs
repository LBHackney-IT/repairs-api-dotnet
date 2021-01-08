using System;
using System.Collections.Generic;
using NUnit.Framework;
using RepairsApi.V1.Domain.Repair;
using RepairsApi.V1.Gateways;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Generated;
using SitePropertyUnit = RepairsApi.V1.Domain.Repair.SitePropertyUnit;
using WorkClass = RepairsApi.V1.Domain.Repair.WorkClass;
using WorkElement = RepairsApi.V1.Domain.Repair.WorkElement;

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
            InMemoryDb.Instance.WorkOrders.Should().ContainSingle().Which.IsSameOrEqualTo(expected.ToDb());
        }

        private static WorkOrder CreateWorkOrder()
        {

            var expected = new WorkOrder
            {
                WorkPriority = new WorkPriority
                {
                    PriorityCode = "priorityCode",
                    RequiredCompletionDateTime = DateTime.UtcNow
                },
                WorkClass = new WorkClass
                {
                    WorkClassCode = WorkClassCode._0
                },
                WorkElements = new List<WorkElement>(),
                DescriptionOfWork = "description",
                SitePropertyUnits = new List<SitePropertyUnit>()
            };
            return expected;
        }
    }
}
