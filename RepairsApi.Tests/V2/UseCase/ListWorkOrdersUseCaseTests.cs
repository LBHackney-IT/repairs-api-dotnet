using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers.Parameters;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Extensions;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    [TestFixture]
    public class ListWorkOrdersUseCaseTests
    {
        private ListWorkOrdersUseCase _classUnderTest;
        private MockRepairsGateway _repairsMock;
        private Generator<WorkOrder> _generator;

        [SetUp]
        public void Setup()
        {
            ConfigureGenerator();
            _repairsMock = new MockRepairsGateway();
            _classUnderTest = new ListWorkOrdersUseCase(_repairsMock.Object, CreateFilterBuilder());
        }

        private static FilterBuilder<WorkOrderSearchParameters, WorkOrder> CreateFilterBuilder()
        {
            return new FilterBuilder<WorkOrderSearchParameters, WorkOrder>()
                .AddFilter(
                    searchParams => searchParams.ContractorReference,
                    contractorReference => !string.IsNullOrWhiteSpace(contractorReference),
                    contractorReference => wo => wo.AssignedToPrimary.ContractorReference == contractorReference
                )
                .AddFilter(
                    searchParams => searchParams.PropertyReference,
                    p => !string.IsNullOrWhiteSpace(p),
                    p => wo => wo.Site.PropertyClass.Any(pc => pc.PropertyReference == p)
                )
                .AddFilter(
                    searchParams => searchParams.StatusCode,
                    codes => codes?.All(code => code > 0 && Enum.IsDefined(typeof(WorkStatusCode), code)) ?? false,
                    codes => wo => codes.Select(c => Enum.Parse<WorkStatusCode>(c.ToString())).Contains(wo.StatusCode)
                );
        }

        private void ConfigureGenerator()
        {
            _generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();
        }

        [Test]
        public async Task ReturnsWorkOrders()
        {
            //Arrange
            const int expectedWorkOrderCount = 5;
            GenerateAndReturnWorkOrders(expectedWorkOrderCount);

            //Act
            var workOrders = await _classUnderTest.Execute(new WorkOrderSearchParameters());

            //Assert
            workOrders.Should().HaveCount(expectedWorkOrderCount);
        }

        [Test]
        public async Task HasCorrectFieldsSet()
        {
            //Arrange
            var expectedWorkOrder = GenerateAndReturnWorkOrders(1).Single();
            PropertyClass propertyClass = expectedWorkOrder.Site?.PropertyClass.FirstOrDefault();
            string addressLine = propertyClass?.Address?.AddressLine;
            var expectedResult = new WorkOrderListItem
            {
                Reference = expectedWorkOrder.Id,
                Description = expectedWorkOrder.DescriptionOfWork,
                Owner = expectedWorkOrder.AssignedToPrimary?.Name,
                Priority = expectedWorkOrder.WorkPriority.PriorityDescription,
                Property = addressLine,
                DateRaised = expectedWorkOrder.DateRaised,
                LastUpdated = null,
                PropertyReference = expectedWorkOrder.Site?.PropertyClass.FirstOrDefault()?.PropertyReference,
                TradeCode = expectedWorkOrder.WorkElements.FirstOrDefault()?.Trade.FirstOrDefault()?.CustomCode,
                TradeDescription = expectedWorkOrder.WorkElements.FirstOrDefault()?.Trade.FirstOrDefault()?.CustomName,
                Status = WorkOrderStatus.InProgress
            };

            //Act
            var workOrders = await _classUnderTest.Execute(new WorkOrderSearchParameters());

            //Assert
            workOrders.Single().Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task CanFilterByPropertyRef()
        {
            // Arrange
            var expectedPropertyRef = Guid.NewGuid().ToString();

            var allWorkOrders = _generator.GenerateList(3);
            SetPropertyRefs(allWorkOrders, "not" + expectedPropertyRef);

            var expectedWorkOrders = _generator.GenerateList(3);
            SetPropertyRefs(expectedWorkOrders, expectedPropertyRef);

            allWorkOrders.AddRange(expectedWorkOrders);

            _repairsMock.ReturnsWorkOrders(allWorkOrders);

            var workOrderSearchParameters = new WorkOrderSearchParameters
            {
                PropertyReference = expectedPropertyRef
            };

            // Act
            var workOrders = await _classUnderTest.Execute(workOrderSearchParameters);

            // Assert
            var expectedResponses = expectedWorkOrders.Select(ewo => ewo.ToListItem());
            workOrders.Should().BeEquivalentTo(expectedResponses);
        }

        private static void SetPropertyRefs(List<WorkOrder> expectedWorkOrders, string newRef)
        {
            foreach (var expectedWorkOrder in expectedWorkOrders)
            {
                foreach (var propertyClass in expectedWorkOrder.Site.PropertyClass)
                {
                    propertyClass.PropertyReference = newRef;
                }
            }
        }

        [Test]
        public async Task ReturnsCorrectPageOfWorkOrders()
        {
            //Arrange
            const int workOrderCount = 50;
            const int expectedPageSize = 10;
            var generatedWorkOrders = GenerateAndReturnWorkOrdersWithRandomStatus(workOrderCount);
            var workOrderSearchParameters = new WorkOrderSearchParameters
            {
                PageNumber = 2,
                PageSize = expectedPageSize
            };
            var statusOrder = new[] {
                WorkOrderStatus.InProgress,
                WorkOrderStatus.PendApp,
                WorkOrderStatus.Cancelled,
                WorkOrderStatus.Complete,
                WorkOrderStatus.Unknown
            };

            //Act
            var workOrders = await _classUnderTest.Execute(workOrderSearchParameters);

            //Assert
            var expectedResult = generatedWorkOrders
                .OrderBy(wo => Array.IndexOf(statusOrder, wo.GetStatus()))
                .ThenByDescending(wo => wo.DateRaised)
                .Skip((workOrderSearchParameters.PageNumber - 1) * workOrderSearchParameters.PageSize)
                .Take(workOrderSearchParameters.PageSize)
                .Select(wo => wo.ToListItem());
            workOrders.Should().BeEquivalentTo(expectedResult);
        }

        private List<WorkOrder> GenerateAndReturnWorkOrders(int workOrderCount)
        {

            var generatedWorkOrders = _generator.GenerateList(workOrderCount);

            _repairsMock.Setup(r => r.GetWorkOrders(It.IsAny<IFilter<WorkOrder>>()))
                .ReturnsAsync(generatedWorkOrders);
            return generatedWorkOrders;
        }

        private List<WorkOrder> GenerateAndReturnWorkOrdersWithRandomStatus(int workOrderCount)
        {

            var generator = new Generator<WorkOrder>()
                .AddDefaultGenerators()
                .AddValue(null, (WorkOrder wo) => wo.WorkOrderComplete)
                .AddValue(null, (WorkOrder wo) => wo.JobStatusUpdates)
                .AddGenerator(new RandomEnumGenerator<WorkStatusCode>());

            var generatedWorkOrders = generator.GenerateList(workOrderCount);

            _repairsMock.Setup(r => r.GetWorkOrders(It.IsAny<IFilter<WorkOrder>>()))
                .ReturnsAsync(generatedWorkOrders);
            return generatedWorkOrders;
        }
    }
}
