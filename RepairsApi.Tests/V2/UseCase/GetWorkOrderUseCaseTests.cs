using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    public class GetWorkOrderUseCaseTests
    {
        private MockRepairsGateway _repairsGatewayMock;
        private GetWorkOrderUseCase _classUnderTest;
        private Generator<WorkOrder> _generator;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new MockRepairsGateway();
            _classUnderTest = new GetWorkOrderUseCase(_repairsGatewayMock.Object);
            configureGenerator();
        }

        [Test]
        public async Task ReturnsResponse()
        {
            // arrange
            var generatedWorkOrders = _generator.GenerateList(5);
            var expectedWorkOrder = _generator.Generate();
            expectedWorkOrder.Id = 7;
            generatedWorkOrders.Add(expectedWorkOrder);
            _repairsGatewayMock.ReturnsWorkOrders(generatedWorkOrders);

            // act
            var response = await _classUnderTest.Execute(expectedWorkOrder.Id);

            // assert
            response.Should().BeEquivalentTo(expectedWorkOrder.ToResponse());
        }

        private void configureGenerator()
        {
            _generator = new Generator<WorkOrder>()
                .AddWorkOrderGenerators();
        }
    }
}
