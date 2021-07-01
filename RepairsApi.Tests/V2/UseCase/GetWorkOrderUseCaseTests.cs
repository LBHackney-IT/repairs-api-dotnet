using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2;
using RepairsApi.V2.Domain;
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
        private Mock<IFeatureManager> _featureManager;
        private Mock<IDrsService> _drsService;
        private Mock<IScheduleOfRatesGateway> _sorGatewayMock;

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
            _featureManager = new Mock<IFeatureManager>();
            _drsService = new Mock<IDrsService>();
            _sorGatewayMock = new Mock<IScheduleOfRatesGateway>();

            _classUnderTest = new GetWorkOrderUseCase(
                _repairsGatewayMock.Object,
                _appointmentsGatewayMock.Object,
                Options.Create(_drsOptions),
                _featureManager.Object,
                _drsService.Object,
                _sorGatewayMock.Object
                );
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

        [TestCase(true, true, true, true)]
        [TestCase(false, true, true, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, true, false, false)]
        public async Task UpdatesAssignedOperativesWhenFeatureFlagSetAndContractorUsesDrs(bool updateFeatureEnabled, bool drsFeatureEnabled, bool contractorUsesDrs, bool shouldAssign)
        {
            var expectedWorkOrder = _generator.Generate();
            _repairsGatewayMock.ReturnsWorkOrders(expectedWorkOrder);
            _sorGatewayMock.Setup(x => x.GetContractor(expectedWorkOrder.AssignedToPrimary.ContractorReference))
                .ReturnsAsync(new Contractor
                {
                    UseExternalScheduleManager = contractorUsesDrs
                });
            _featureManager.Setup(x => x.IsEnabledAsync(FeatureFlags.UpdateOperativesOnWorkOrderGet))
                .ReturnsAsync(updateFeatureEnabled);
            _featureManager.Setup(x => x.IsEnabledAsync(FeatureFlags.DRSIntegration))
                .ReturnsAsync(drsFeatureEnabled);

            await _classUnderTest.Execute(expectedWorkOrder.Id);

            _drsService.Verify(x => x.UpdateAssignedOperative(expectedWorkOrder.Id), shouldAssign ? Times.Once() : Times.Never());
        }

        private void ConfigureGenerator()
        {
            _generator = new Generator<WorkOrder>()
                .AddWorkOrderGenerators();
        }
    }
}
