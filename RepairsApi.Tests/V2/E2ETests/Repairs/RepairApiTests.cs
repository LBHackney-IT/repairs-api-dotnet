using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RepairsApi.V2.UseCase;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;
using Quantity = RepairsApi.V2.Generated.Quantity;
using RateScheduleItem = RepairsApi.V2.Generated.RateScheduleItem;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;
using RepairsApi.V2.Services;

namespace RepairsApi.Tests.V2.E2ETests.Repairs
{
    public partial class RepairApiTests : MockWebApplicationFactory
    {
        [SetUp]
        public void SetUp()
        {
            SetUserRole(UserGroups.Agent);
        }

        [Test]
        public async Task ScheduleRepair()
        {
            // Arrange
            var request = GenerateWorkOrder<ScheduleRepair>()
                .AddValue(new List<double> { 1 }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
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
            var result = await CreateWorkOrder(wo => wo.AssignedToPrimary.Organization.Reference.First().ID = TestDataSeeder.DRSContractor);

            result.ExternallyManagedAppointment.Should().BeTrue();
            result.ExternalAppointmentManagementUrl.Query.Should().Contain($"tokenId={SoapMock.ExpectedToken}");

            SoapMock.Verify(s => s.createOrderAsync(It.IsAny<V2_Generated_DRS.createOrder>()));
        }

        [Test]
        public async Task DeletesFromDRS()
        {
            var result = await CreateWorkOrder(wo => wo.AssignedToPrimary.Organization.Reference.First().ID = TestDataSeeder.DRSContractor);

            await CancelWorkOrder(result.Id);

            SoapMock.Verify(s => s.deleteOrderAsync(It.IsAny<V2_Generated_DRS.deleteOrder>()));
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
        public async Task GetFilteredListOfWorkOrders_Status()
        {
            // Arrange
            var openWorkOrderResult = await CreateWorkOrder();
            var completedWorkOrderResult = await CreateWorkOrder();
            var code = await CancelWorkOrder(completedWorkOrderResult.Id);

            // Act
            var (openCode, openResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?StatusCode={(int) WorkStatusCode.Open}&PageSize=50");
            var (closedCode, closedResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?StatusCode={(int) WorkStatusCode.Canceled}&PageSize=50");
            var (multiCode, multiResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?StatusCode={(int) WorkStatusCode.Open}&StatusCode={(int) WorkStatusCode.Canceled}&PageSize=50");

            // Assert
            openCode.Should().Be(HttpStatusCode.OK);
            openResponse.Should().ContainSingle(wo => wo.Reference == openWorkOrderResult.Id);
            openResponse.Should().NotContain(wo => wo.Reference == completedWorkOrderResult.Id);

            closedCode.Should().Be(HttpStatusCode.OK);
            closedResponse.Should().ContainSingle(wo => wo.Reference == completedWorkOrderResult.Id);
            closedResponse.Should().NotContain(wo => wo.Reference == openWorkOrderResult.Id);

            multiCode.Should().Be(HttpStatusCode.OK);
            multiResponse.Should().ContainSingle(wo => wo.Reference == openWorkOrderResult.Id);

            var wo = GetWorkOrderFromDB(completedWorkOrderResult.Id);

            multiResponse.Should().ContainSingle(wo => wo.Reference == completedWorkOrderResult.Id);
        }


        [Test]
        public async Task GetFilteredListOfWorkOrders_Priority()
        {
            // Arrange
            var urgentResult = await CreateWorkOrder(sr => sr.Priority.PriorityCode = 3);
            var immediateResult = await CreateWorkOrder(sr => sr.Priority.PriorityCode = 0);

            // Act
            var (urgentCode, urgentResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?Priorities={3}");
            var (immediateCode, immediateResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?Priorities={0}");
            var (multiCode, multiResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?Priorities={3}&Priorities={0}");

            // Assert
            urgentCode.Should().Be(HttpStatusCode.OK);
            urgentResponse.Should().ContainSingle(wo => wo.Reference == urgentResult.Id);
            urgentResponse.Should().NotContain(wo => wo.Reference == immediateResult.Id);

            immediateCode.Should().Be(HttpStatusCode.OK);
            immediateResponse.Should().ContainSingle(wo => wo.Reference == immediateResult.Id);
            immediateResponse.Should().NotContain(wo => wo.Reference == urgentResult.Id);

            multiCode.Should().Be(HttpStatusCode.OK);
            multiResponse.Should().ContainSingle(wo => wo.Reference == immediateResult.Id);
            multiResponse.Should().ContainSingle(wo => wo.Reference == urgentResult.Id);
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
            var request = new Generator<WorkOrderComplete>()
                            .AddWorkOrderCompleteGenerators()
                            .AddValue(result.Id.ToString(), (WorkOrderComplete woc) => woc.WorkOrderReference.ID)
                            .SetListLength<JobStatusUpdates>(1)
                            .AddValue(JobStatusUpdateTypeCode._0, (JobStatusUpdates jsu) => jsu.TypeCode)
                            .AddValue(CustomJobStatusUpdates.Completed, (JobStatusUpdates jsu) => jsu.OtherType)
                            .Generate();

            // Act
            SetUserRole(UserGroups.Contractor);
            var code = await Post("/api/v2/workOrderComplete", request);
            var workOrder = GetWorkOrderFromDB(result.Id);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            workOrder.StatusCode.Should().Be(WorkStatusCode.Complete);
            NotifyMock.SentMails.Should().ContainSingle(m => m[WorkOrderCompletedEmailVariables.WorkOrderId].ToString() == result.Id.ToString());
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
            await UpdateSorCodes(result.Id, req =>
            {
                req.Comments = expectedNote;
            });

            // Act
            var (code, notes) = await Get<IList<NoteListItem>>($"/api/v2/workOrders/{result.Id}/notes");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            notes.Should().ContainSingle(note => note.Note == expectedNote);
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
                        Amount = new List<double>() { task.Quantity }
                    }
                }).ToList()
            };
        }

        private void AddTestCode(string expectedCode)
        {
            using var ctx = GetContext();
            TestDataSeeder.AddCode(ctx.DB, expectedCode);
        }

        private Generator<T> GenerateWorkOrder<T>()
        {
            Generator<T> gen = new Generator<T>();

            using (var ctx = GetContext())
            {
                var db = ctx.DB;
                gen = new Generator<T>()
                                .AddWorkOrderGenerators()
                                .AddValue(new List<double> { 0 }, (RateScheduleItem rsi) => rsi.Quantity.Amount);
            };

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
            var request = new Generator<WorkOrderComplete>()
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
            JobStatusUpdate request = new Generator<JobStatusUpdate>()
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
                .AddValue(new List<double> { 1 }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
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
            return new Generator<JobStatusUpdate>()
                .AddJobStatusUpdateGenerators()
                .AddValue(JobStatusUpdateTypeCode._80, (JobStatusUpdate jsu) => jsu.TypeCode)
                .AddValue(workOrderId.ToString(), (JobStatusUpdate jsu) => jsu.RelatedWorkOrderReference.ID)
                .AddValue(workElement, (JobStatusUpdate jsu) => jsu.MoreSpecificSORCode)
                .AddValue("comments", (JobStatusUpdate jsu) => jsu.Comments)
                .Generate();
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
                    Amount = new double[] { quantity }
                }
            });
        }
    }
}
