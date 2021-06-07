using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using V2_Generated_DRS;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;
using Quantity = RepairsApi.V2.Generated.Quantity;
using RateScheduleItem = RepairsApi.V2.Generated.RateScheduleItem;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;

namespace RepairsApi.Tests.E2ETests.Repairs
{
    public partial class RepairApiTests : MockWebApplicationFactory
    {
        private readonly Fixture _fixture;

        public RepairApiTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public void SetUp()
        {
            SetUserRole(UserGroups.Agent);
        }

        [TestCase(UserGroups.Agent)]
        [TestCase(UserGroups.AuthorisationManager)]
        public async Task ScheduleRepair(string userGroup)
        {
            // Arrange
            SetUserRole(userGroup);
            var request = GenerateWorkOrder<ScheduleRepair>()
                .AddValue(new List<double>
                {
                    1
                }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                .Generate();

            // Act
            var (code, response) = await Post<CreateOrderResult>("/api/v2/workOrders/schedule", request);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            var wo = GetWorkOrderFromDB(response.Id);
            wo.StatusCode.Should().Be(WorkStatusCode.Open);
            wo.WorkPriority.NumberOfDays.Should().Be(request.Priority.NumberOfDays);
        }

        [Test]
        public async Task ForwardedToDRS()
        {
            SetupSoapMock();

            var result = await CreateWorkOrder(wo => wo.AssignedToPrimary.Organization.Reference.First().ID = TestDataSeeder.DRSContractor);

            result.ExternallyManagedAppointment.Should().BeTrue();
            result.ExternalAppointmentManagementUrl.Query.Should().Contain($"tokenId={SoapMock.ExpectedToken}");

            SoapMock.Verify(s => s.createOrderAsync(It.IsAny<V2_Generated_DRS.createOrder>()));
        }

        [Test]
        public async Task DeletesFromDRS()
        {
            SetupSoapMock();

            var result = await CreateWorkOrder(wo => wo.AssignedToPrimary.Organization.Reference.First().ID = TestDataSeeder.DRSContractor);

            await CancelWorkOrder(result.Id);

            SoapMock.Verify(s => s.deleteOrderAsync(It.IsAny<V2_Generated_DRS.deleteOrder>()));
        }

        [Test]
        public async Task CompletedInDRS()
        {
            
            SetUserRole(UserGroups.ContractManager);
            var drsOrder = _fixture.Create<order>();
            drsOrder.status = orderStatus.PLANNED;
            SoapMock.Setup(s => s.selectOrderAsync(It.IsAny<selectOrder>()))
                .ReturnsAsync(new selectOrderResponse
                {
                    @return = new xmbSelectOrderResponse
                    {
                        status = responseStatus.success,
                        theOrders = new[]
                        {
                            drsOrder
                        }
                    }
                });
            SoapMock.Setup(s => s.updateBookingAsync(It.IsAny<updateBooking>()))
                .ReturnsAsync(new updateBookingResponse
                {
                    @return = new xmbUpdateBookingResponse
                    {
                        status = responseStatus.success
                    }
                });

            var result = await CreateWorkOrder(wo => wo.AssignedToPrimary.Organization.Reference.First().ID = TestDataSeeder.DRSContractor);
            await CompleteWorkOrder(result.Id);

            SoapMock.Verify(s => s.updateBookingAsync(It.IsAny<updateBooking>()));
        }

        [Test]
        public async Task ViewElements()
        {
            var expectedName = "Expected Name";
            var result = await CreateWorkOrder(req =>
            {
                req.WorkElement.First().RateScheduleItem.First().CustomName = expectedName;
            });

            var (code, response) = await Get<IEnumerable<WorkOrderItemViewModel>>($"/api/v2/workOrders/{result.Id}/tasks");

            code.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeEmpty();
            response.Should().Contain(item => item.Description == expectedName);
            response.Should().Contain(item => item.OriginalQuantity.HasValue);
        }

        [Test]
        public async Task BadRequestWhenMultipleAmountsProvided()
        {
            // Arrange
            var request = GenerateWorkOrder<ScheduleRepair>().Generate();
            request.WorkElement.First().RateScheduleItem.First().Quantity.Amount.Add(3.5);

            // Act
            var code = await Post("/api/v2/workOrders/schedule", request);

            // Assert
            code.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetListOfWorkOrders()
        {
            // Arrange
            var result = await CreateWorkOrder();

            // Act
            var (code, response) = await Get<List<WorkOrderListItem>>("/api/v2/workOrders");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().Contain(wo => wo.Reference == result.Id);
        }

        [Test]
        public async Task GetWorkOrder()
        {
            // Arrange
            const string tradeName = "trade name";
            const string contractor = "contractor name";
            var result = await CreateWorkOrder(req =>
            {
                req.WorkElement.First().Trade.First().CustomName = tradeName;
                req.AssignedToPrimary.Name = contractor;
            });

            var (code, response) = await Get<RepairsApi.V2.Boundary.WorkOrderResponse>($"/api/v2/workOrders/{result.Id}");
            code.Should().Be(HttpStatusCode.OK);
            response.TradeDescription.Should().Be(tradeName);
            response.Owner.Should().Be(contractor);
        }

        [Test]
        public async Task CompleteWorkOrder()
        {
            // Arrange
            var result = await CreateWorkOrder();
            SetUserRole(UserGroups.Contractor);
            var code = await CompleteWorkOrder(result.Id);
            var workOrder = GetWorkOrderFromDB(result.Id);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            workOrder.StatusCode.Should().Be(WorkStatusCode.Complete);
        }

        private async Task<HttpStatusCode> CompleteWorkOrder(int workOrderId)
        {
            var request = new Helpers.StubGeneration.Generator<WorkOrderComplete>()
                .AddWorkOrderCompleteGenerators()
                .AddValue(workOrderId.ToString(), (WorkOrderComplete woc) => woc.WorkOrderReference.ID)
                .SetListLength<JobStatusUpdates>(1)
                .AddValue(JobStatusUpdateTypeCode._0, (JobStatusUpdates jsu) => jsu.TypeCode)
                .AddValue(CustomJobStatusUpdates.Completed, (JobStatusUpdates jsu) => jsu.OtherType)
                .Generate();

            // Act
            var response = await Post("/api/v2/workOrderComplete", request);
            return response;
        }

        [Test]
        public async Task CompleteRepairTwice()
        {
            // Arrange
            var result = await CreateWorkOrder();
            await CancelWorkOrder(result.Id);

            // Act
            var secondResponseCode = await CancelWorkOrder(result.Id);

            // Assert
            secondResponseCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task CreateDecimalQuantity()
        {
            // Arrange
            var result = await CreateWorkOrder(wo =>
            {
                wo.WorkElement.First().RateScheduleItem.First().Quantity.Amount = 5.5.MakeArray();
            });

            // Act
            var tasks = await GetTasks(result.Id);

            // Assert
            tasks.Should().ContainSingle(t => t.Quantity == 5.5);
        }

        [Test]
        public async Task UpdateSorCodes()
        {
            // Arrange
            string expectedCode = "expectedCode_UpdateWorkOrder";
            AddTestCode(expectedCode);
            var result = await CreateWorkOrder();
            var originalTasks = await GetTasks(result.Id);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(originalTasks);
            AddRateScheduleItem(workElement, expectedCode, 10);
            JobStatusUpdate request = CreateUpdateRequest(result.Id, workElement);

            // Act
            var code = await Post("/api/v2/jobStatusUpdate", request);
            var newTasks = await GetTasks(result.Id);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            newTasks.Count().Should().Be(originalTasks.Count() + 1);
        }

        [Test]
        public async Task CanViewNotes()
        {
            // Arrange
            var expectedNote = "expectedComments";
            var result = await CreateWorkOrder();
            await UpdateJob(result.Id, req =>
            {
                req.Comments = expectedNote;
                req.TypeCode = JobStatusUpdateTypeCode._0;
                req.OtherType = CustomJobStatusUpdates.AddNote;
            });

            // Act
            var (code, notes) = await Get<IList<NoteListItem>>($"/api/v2/workOrders/{result.Id}/notes");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            notes.Should().ContainSingle(note => note.Note.Contains(expectedNote));
        }

        [Test]
        public async Task GetMissingWorkOrder()
        {
            // Arrange
            var expectedId = 1000;
            // Act
            var (code, response) = await Get<string>($"/api/v2/workOrders/{expectedId}");

            // Assert
            code.Should().Be(404);
            response.Should().Be($"Unable to locate work order {expectedId}");
        }

        [TestCase(JobStatusUpdateTypeCode._120, WorkStatusCode.Hold)]
        [TestCase(JobStatusUpdateTypeCode._12020, WorkStatusCode.PendMaterial)]
        public async Task HoldAndResumeWorkOrder(JobStatusUpdateTypeCode updateCode, WorkStatusCode workOrderHoldCode)
        {
            // Arrange
            var result = await CreateWorkOrder();

            // Act
            await UpdateSorCodes(result.Id, req => req.TypeCode = updateCode);
            var heldOrder = GetWorkOrderFromDB(result.Id);
            await UpdateSorCodes(result.Id, req =>
            {
                req.TypeCode = JobStatusUpdateTypeCode._0;
                req.OtherType = CustomJobStatusUpdates.Resume;
            });
            var resumedOrder = GetWorkOrderFromDB(result.Id);

            // Assert
            heldOrder.StatusCode.Should().Be(workOrderHoldCode);
            resumedOrder.StatusCode.Should().Be(WorkStatusCode.Open);
        }

        private static RepairsApi.V2.Generated.WorkElement TransformTasksToWorkElement(IEnumerable<WorkOrderItemViewModel> tasks)
        {
            return new RepairsApi.V2.Generated.WorkElement
            {
                RateScheduleItem = tasks.Select(task => new RateScheduleItem
                {
                    Id = task.Id,
                    CustomCode = task.Code,
                    CustomName = task.Description,
                    Quantity = new Quantity
                    {
                        Amount = new List<double>()
                        {
                            task.Quantity
                        }
                    }
                }).ToList()
            };
        }

        private void AddTestCode(string expectedCode)
        {
            using var ctx = GetContext();
            TestDataSeeder.AddCode(ctx.DB, expectedCode);
        }

        private Helpers.StubGeneration.Generator<T> GenerateWorkOrder<T>()
        {
            Helpers.StubGeneration.Generator<T> gen = new Helpers.StubGeneration.Generator<T>();

            using (var ctx = GetContext())
            {
                var db = ctx.DB;
                gen = new Helpers.StubGeneration.Generator<T>()
                    .AddWorkOrderGenerators()
                    .AddValue(new List<double>
                    {
                        0
                    }, (RateScheduleItem rsi) => rsi.Quantity.Amount);
            }
            ;

            return gen;
        }

        public WorkOrder GetWorkOrderFromDB(int id, Action<WorkOrder> modifier = null)
        {
            using var ctx = GetContext();
            var db = ctx.DB;
            var repair = db.WorkOrders.Find(id);
            modifier?.Invoke(repair);
            return repair;
        }

        public async Task<HttpStatusCode> CancelWorkOrder(int id)
        {
            var request = new Helpers.StubGeneration.Generator<WorkOrderComplete>()
                .AddWorkOrderCompleteGenerators()
                .SetListLength<JobStatusUpdates>(1)
                .AddValue(id.ToString(), (WorkOrderComplete woc) => woc.WorkOrderReference.ID)
                .AddValue(JobStatusUpdateTypeCode._0, (JobStatusUpdates jsu) => jsu.TypeCode)
                .AddValue(CustomJobStatusUpdates.Cancelled, (JobStatusUpdates jsu) => jsu.OtherType)
                .Generate();

            return await Post("/api/v2/workOrderComplete", request);
        }

        private Task<HttpStatusCode> UpdateJob(int workOrderId, JobStatusUpdateTypeCode typeCode)
        {
            return UpdateJob(workOrderId, jsu => jsu.TypeCode = typeCode);
        }

        private async Task<HttpStatusCode> UpdateSorCodes(int workOrderId, Action<JobStatusUpdate> interceptor = null)
        {
            var tasks = await GetTasks(workOrderId);
            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            return await UpdateJob(workOrderId, jsu =>
            {
                jsu.MoreSpecificSORCode = workElement;
                interceptor?.Invoke(jsu);
            });
        }

        private async Task<HttpStatusCode> UpdateJob(int workOrderId, Action<JobStatusUpdate> interceptor = null)
        {
            JobStatusUpdate request = new Helpers.StubGeneration.Generator<JobStatusUpdate>()
                .AddJobStatusUpdateGenerators()
                .AddValue(JobStatusUpdateTypeCode._80, (JobStatusUpdate jsu) => jsu.TypeCode)
                .AddValue(workOrderId.ToString(), (JobStatusUpdate jsu) => jsu.RelatedWorkOrderReference.ID)
                .AddValue("comments", (JobStatusUpdate jsu) => jsu.Comments)
                .Generate();

            interceptor?.Invoke(request);

            return await Post("/api/v2/jobStatusUpdate", request);
        }

        private async Task<CreateOrderResult> CreateWorkOrder(Action<ScheduleRepair> interceptor = null)
        {
            var request = GenerateWorkOrder<ScheduleRepair>()
                .AddValue(new List<double>
                {
                    1
                }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                .Generate();

            interceptor?.Invoke(request);

            var (_, response) = await Post<CreateOrderResult>("/api/v2/workOrders/schedule", request);

            return response;
        }

        public async Task<IEnumerable<WorkOrderItemViewModel>> GetTasks(int workOrderId)
        {
            var (_, response) = await Get<IEnumerable<WorkOrderItemViewModel>>($"/api/v2/workOrders/{workOrderId}/tasks");

            return response;
        }

        private static JobStatusUpdate CreateUpdateRequest(int workOrderId, RepairsApi.V2.Generated.WorkElement workElement)
        {
            return new Helpers.StubGeneration.Generator<JobStatusUpdate>()
                .AddJobStatusUpdateGenerators()
                .AddValue(JobStatusUpdateTypeCode._80, (JobStatusUpdate jsu) => jsu.TypeCode)
                .AddValue(workOrderId.ToString(), (JobStatusUpdate jsu) => jsu.RelatedWorkOrderReference.ID)
                .AddValue(workElement, (JobStatusUpdate jsu) => jsu.MoreSpecificSORCode)
                .AddValue("comments", (JobStatusUpdate jsu) => jsu.Comments)
                .Generate();
        }

        private void SetupSoapMock()
        {
            SoapMock.Setup(s => s.updateBookingAsync(It.IsAny<updateBooking>()))
                .ReturnsAsync(new updateBookingResponse
                {
                    @return = new xmbUpdateBookingResponse
                    {
                        status = responseStatus.success
                    }
                });

        }

        private static void AddRateScheduleItem(RepairsApi.V2.Generated.WorkElement workElement, string code, int quantity, string id = null)
        {
            workElement.RateScheduleItem.Add(new RateScheduleItem
            {
                Id = id,
                CustomCode = code,
                CustomName = "test code",
                Quantity = new Quantity
                {
                    Amount = new double[]
                    {
                        quantity
                    }
                }
            });
        }
    }
}
