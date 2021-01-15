using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.V2.Gateways
{
    public class WorkOrderCompletionGatewayTests
    {
        private WorkOrderCompletionGateway _classUnderTest;
        private Generator<WorkOrderComplete> _generator;

        [SetUp]
        public void Setup()
        {
            SetupGenerator();
            _classUnderTest = new WorkOrderCompletionGateway(InMemoryDb.Instance);
        }

        private void SetupGenerator()
        {
            _generator = new Generator<WorkOrderComplete>(new Dictionary<Type, IGenerator>
            {
                {
                    typeof(string), new RandomStringGenerator(10)
                },
                {
                    typeof(double), new RandomDoubleGenerator(0, 50)
                },
                {
                    typeof(bool), new RandomBoolGenerator()
                }
            });
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task CanCreateWorkOrderCompletion()
        {
            // arrange
            var expected = CreateWorkOrderCompletion();

            // act
            await _classUnderTest.CreateWorkOrderCompletion(expected);
            await InMemoryDb.Instance.SaveChangesAsync();

            // assert
            InMemoryDb.Instance.WorkOrderCompletes.Should().ContainSingle().Which.IsSameOrEqualTo(expected);
        }

        private WorkOrderComplete CreateWorkOrderCompletion()
        {
            return _generator.Generate();
        }
    }
}
