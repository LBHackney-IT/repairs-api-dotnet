using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    public class GetWorkOrderUseCaseTests
    {
        private MockRepairsGateway _repairsGatewayMock;
        private Mock<IAppointmentsGateway> _appointmentsGatewayMock;
        private GetWorkOrderUseCase _classUnderTest;
        private Generator<WorkOrder> _generator;
        private DrsOptions _drsOptions;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new MockRepairsGateway();
            _appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            _drsOptions = new DrsOptions
            {
                Login = "login",
                Password = "password",
                APIAddress = new Uri("https://apiAddress.none"),
                ManagementAddress = new Uri("https://managementAddress.none")
            };
            _classUnderTest = new GetWorkOrderUseCase(_repairsGatewayMock.Object, _appointmentsGatewayMock.Object, Options.Create(_drsOptions));
            ConfigureGenerator();
        }

        [Test]
        public async Task ReturnsResponse()
        {
            // arrange
            var generatedWorkOrders = _generator.GenerateList(5);
            var expectedWorkOrder = _generator.Generate();
            expectedWorkOrder.Id = 7;
            generatedWorkOrders.Add(expectedWorkOrder);
            AppointmentDetails appointment = new AppointmentDetails
            {
                Date = DateTime.UtcNow,
                Description = "test",
                End = DateTime.UtcNow,
                Start = DateTime.UtcNow
            };
            _repairsGatewayMock.ReturnsWorkOrders(generatedWorkOrders);
            _appointmentsGatewayMock.Setup(a => a.GetAppointment(It.IsAny<int>())).ReturnsAsync(appointment);

            // act
            var response = await _classUnderTest.Execute(expectedWorkOrder.Id);

            // assert
            response.Should().BeEquivalentTo(expectedWorkOrder.ToResponse(appointment, _drsOptions.ManagementAddress));
        }


        [Test]
        public async Task MapsNullAppointment()
        {
            // arrange
            var generatedWorkOrders = _generator.GenerateList(5);
            var expectedWorkOrder = _generator.Generate();
            expectedWorkOrder.Id = 7;
            generatedWorkOrders.Add(expectedWorkOrder);
            _repairsGatewayMock.ReturnsWorkOrders(generatedWorkOrders);
            _appointmentsGatewayMock.Setup(a => a.GetAppointment(It.IsAny<int>())).ReturnsAsync((AppointmentDetails) null);

            // act
            var response = await _classUnderTest.Execute(expectedWorkOrder.Id);

            // assert
            response.Should().BeEquivalentTo(expectedWorkOrder.ToResponse(null, _drsOptions.ManagementAddress));
        }

        private void ConfigureGenerator()
        {
            _generator = new Generator<WorkOrder>()
                .AddWorkOrderGenerators();
        }
    }
}
