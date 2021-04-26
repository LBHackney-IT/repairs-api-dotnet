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
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;
using Quantity = RepairsApi.V2.Generated.Quantity;
using RateScheduleItem = RepairsApi.V2.Generated.RateScheduleItem;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;

namespace RepairsApi.Tests.V2.E2ETests.Repairs
{
    [SingleThreaded]
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
            var (code, response) = await Post<int>("/api/v2/workOrders/schedule", request);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            var wo = GetWorkOrderFromDB(response);
            wo.StatusCode.Should().Be(WorkStatusCode.Open);
            wo.WorkPriority.NumberOfDays.Should().Be(request.Priority.NumberOfDays);
        }

        [Test]
        public async Task ForwardedToDRS()
        {
            var id = await CreateWorkOrder(wo => wo.AssignedToPrimary.Organization.Reference.First().ID = TestDataSeeder.DRSContractor);

            SoapMock.Verify(s => s.createOrderAsync(It.IsAny<V2_Generated_DRS.createOrder>()));
        }

        [Test]
        public async Task ViewElements()
        {
            var expectedName = "Expected Name";
            var id = await CreateWorkOrder(req =>
            {
                req.WorkElement.First().RateScheduleItem.First().CustomName = expectedName;
            });

            var (code, response) = await Get<IEnumerable<WorkOrderItemViewModel>>($"/api/v2/workOrders/{id}/tasks");

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
            var workOrderId = await CreateWorkOrder();

            // Act
            var (code, response) = await Get<List<WorkOrderListItem>>("/api/v2/workOrders");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().Contain(wo => wo.Reference == workOrderId);
        }

        [Test]
        public async Task GetFilteredListOfWorkOrders_Status()
        {
            // Arrange
            var openWorkOrderId = await CreateWorkOrder();
            var completedWorkOrderId = await CreateWorkOrder();
            var code = await CancelWorkOrder(completedWorkOrderId);

            // Act
            var (openCode, openResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?StatusCode={(int) WorkStatusCode.Open}");
            var (closedCode, closedResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?StatusCode={(int) WorkStatusCode.Canceled}");
            var (multiCode, multiResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?StatusCode={(int) WorkStatusCode.Open}&StatusCode={(int) WorkStatusCode.Canceled}");

            // Assert
            openCode.Should().Be(HttpStatusCode.OK);
            openResponse.Should().ContainSingle(wo => wo.Reference == openWorkOrderId);
            openResponse.Should().NotContain(wo => wo.Reference == completedWorkOrderId);

            closedCode.Should().Be(HttpStatusCode.OK);
            closedResponse.Should().ContainSingle(wo => wo.Reference == completedWorkOrderId);
            closedResponse.Should().NotContain(wo => wo.Reference == openWorkOrderId);

            multiCode.Should().Be(HttpStatusCode.OK);
            multiResponse.Should().ContainSingle(wo => wo.Reference == openWorkOrderId);
            multiResponse.Should().ContainSingle(wo => wo.Reference == completedWorkOrderId);
        }


        [Test]
        public async Task GetFilteredListOfWorkOrders_Priority()
        {
            // Arrange
            var urgentWorkOrderId = await CreateWorkOrder(sr => sr.Priority.PriorityCode = 3);
            var immediateWorkOrderId = await CreateWorkOrder(sr => sr.Priority.PriorityCode = 0);

            // Act
            var (urgentCode, urgentResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?Priorities={3}");
            var (immediateCode, immediateResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?Priorities={0}");
            var (multiCode, multiResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?Priorities={3}&Priorities={0}");

            // Assert
            urgentCode.Should().Be(HttpStatusCode.OK);
            urgentResponse.Should().ContainSingle(wo => wo.Reference == urgentWorkOrderId);
            urgentResponse.Should().NotContain(wo => wo.Reference == immediateWorkOrderId);

            immediateCode.Should().Be(HttpStatusCode.OK);
            immediateResponse.Should().ContainSingle(wo => wo.Reference == immediateWorkOrderId);
            immediateResponse.Should().NotContain(wo => wo.Reference == urgentWorkOrderId);

            multiCode.Should().Be(HttpStatusCode.OK);
            multiResponse.Should().ContainSingle(wo => wo.Reference == immediateWorkOrderId);
            multiResponse.Should().ContainSingle(wo => wo.Reference == urgentWorkOrderId);
        }

        [Test]
        public async Task GetWorkOrder()
        {
            // Arrange
            const string tradeName = "trade name";
            var workOrderId = await CreateWorkOrder(req =>
            {
                req.WorkElement.First().Trade.First().CustomName = tradeName;
            });

            var (code, response) = await Get<RepairsApi.V2.Boundary.WorkOrderResponse>($"/api/v2/workOrders/{workOrderId}");
            code.Should().Be(HttpStatusCode.OK);
            response.TradeDescription.Should().Be(tradeName);
        }

        [Test]
        public async Task CompleteWorkOrder()
        {
            // Arrange
            var id = await CreateWorkOrder();
            var request = new Generator<WorkOrderComplete>()
                            .AddWorkOrderCompleteGenerators()
                            .AddValue(id.ToString(), (WorkOrderComplete woc) => woc.WorkOrderReference.ID)
                            .AddValue(JobStatusUpdateTypeCode._0, (JobStatusUpdates jsu) => jsu.TypeCode)
                            .AddValue(CustomJobStatusUpdates.Cancelled, (JobStatusUpdates jsu) => jsu.OtherType)
                            .Generate();

            // Act
            var code = await Post("/api/v2/workOrderComplete", request);
            var workOrder = GetWorkOrderFromDB(id);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            workOrder.StatusCode.Should().Be(WorkStatusCode.Canceled);
        }

        [Test]
        public async Task CompleteRepairTwice()
        {
            // Arrange
            var woId = await CreateWorkOrder();
            await CancelWorkOrder(woId);

            // Act
            var secondResponseCode = await CancelWorkOrder(woId);

            // Assert
            secondResponseCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UpdateSorCodes()
        {
            // Arrange
            string expectedCode = "expectedCode_UpdateWorkOrder";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var originalTasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(originalTasks);
            AddRateScheduleItem(workElement, expectedCode, 10);
            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);

            // Act
            var code = await Post("/api/v2/jobStatusUpdate", request);
            var newTasks = await GetTasks(workOrderId);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            newTasks.Count().Should().Be(originalTasks.Count() + 1);
        }

        [Test]
        public async Task CanViewNotes()
        {
            // Arrange
            var expectedNote = "expectedComments";
            var workOrderId = await CreateWorkOrder();
            await UpdateJob(workOrderId, req =>
            {
                req.Comments = expectedNote;
            });

            // Act
            var (code, notes) = await Get<IList<NoteListItem>>($"/api/v2/workOrders/{workOrderId}/notes");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            notes.Should().ContainSingle(note => note.Note == expectedNote);
        }

        [Test]
        public async Task UpdateCausesPendingApprovalOnWorkOrderWhenCostLimitExceeded()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);

            // Act
            await Post("/api/v2/jobStatusUpdate", request);
            var workOrder = GetWorkOrderWithJobStatusUpdatesFromDB(workOrderId);

            // Assert
            workOrder.StatusCode.Should().Be(WorkStatusCode.PendingVariation);
            workOrder.JobStatusUpdates[0].TypeCode.Should().Be(JobStatusUpdateTypeCode._180);
        }

        [Test]
        public async Task WorkOrderRejectVariation()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate2";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);
            // Act
            await Post("/api/v2/jobStatusUpdate", request);

            //reject variation
            request.TypeCode = JobStatusUpdateTypeCode._125;
            await Post("/api/v2/jobStatusUpdate", request, "contract manager");
            var rejectedOrder = GetWorkOrderFromDB(workOrderId);

            // Assert
            rejectedOrder.StatusCode.Should().Be(WorkStatusCode.VariationRejected);
        }

        [Test]
        public async Task WorkOrderApproveVariation()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate3";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000, "3");

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);
            // Act
            await Post("/api/v2/jobStatusUpdate", request);

            request.TypeCode = JobStatusUpdateTypeCode._10020;
            await Post("/api/v2/jobStatusUpdate", request, "contract manager");
            var approvedOrder = GetWorkOrderWithJobStatusUpdatesFromDB(workOrderId);

            // Assert
            approvedOrder.StatusCode.Should().Be(WorkStatusCode.VariationApproved);
            approvedOrder.JobStatusUpdates[1].Comments.Should().Contain("Approved");
        }

        [Test]
        public async Task ContractorAcknowledgeWorkOrderSetToInProgress()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate4";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);
            // Act
            await Post("/api/v2/jobStatusUpdate", request);

            //approve variation
            request.TypeCode = JobStatusUpdateTypeCode._10020;
            await Post("/api/v2/jobStatusUpdate", request, "contract manager");

            //acknowledge approved variation
            request.TypeCode = JobStatusUpdateTypeCode._10010;
            await Post("/api/v2/jobStatusUpdate", request, "contractor");
            var acknowledgedWorkorder = GetWorkOrderFromDB(workOrderId);

            // Assert
            acknowledgedWorkorder.StatusCode.Should().Be(WorkStatusCode.Open);
        }

        [Test]
        public async Task ContractorWorkOrderApproveVariationReturns401()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate5";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);
            // Act
            await Post("/api/v2/jobStatusUpdate", request);

            //approve variation
            request.TypeCode = JobStatusUpdateTypeCode._10020;
            var response = await Post("/api/v2/jobStatusUpdate", request, "contractor");

            response.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task AgentWorkOrderApproveVariationReturns401()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate6";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);
            // Act
            await Post("/api/v2/jobStatusUpdate", request);

            //approve variation
            request.TypeCode = JobStatusUpdateTypeCode._10020;
            var response = await Post("/api/v2/jobStatusUpdate", request);

            response.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ContractorWorkOrderRejectVariationReturns401()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate7";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);
            // Act
            await Post("/api/v2/jobStatusUpdate", request);

            //reject variation
            request.TypeCode = JobStatusUpdateTypeCode._125;
            var response = await Post("/api/v2/jobStatusUpdate", request, "contractor");

            // Assert
            response.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task AgentWorkOrderRejectVariationReturns401()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate8";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);
            // Act
            await Post("/api/v2/jobStatusUpdate", request);

            //reject variation
            request.TypeCode = JobStatusUpdateTypeCode._125;
            var response = await Post("/api/v2/jobStatusUpdate", request);//, "contractor");

            // Assert
            response.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ContractManagerAcknowledgeWorkOrderSetToInProgressReturns401()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate9";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);
            // Act
            await Post("/api/v2/jobStatusUpdate", request);

            //approve variation
            request.TypeCode = JobStatusUpdateTypeCode._10020;
            await Post("/api/v2/jobStatusUpdate", request, "contract manager");

            //acknowledge approved variation
            request.TypeCode = JobStatusUpdateTypeCode._10010;
            var response = await Post("/api/v2/jobStatusUpdate", request, "contract manager");

            // Assert
            response.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task AgentAcknowledgeWorkOrderSetToInProgressReturns401()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate10";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);
            // Act
            await Post("/api/v2/jobStatusUpdate", request);

            //approve variation
            request.TypeCode = JobStatusUpdateTypeCode._10020;
            await Post("/api/v2/jobStatusUpdate", request, "contract manager");

            //acknowledge approved variation
            request.TypeCode = JobStatusUpdateTypeCode._10010;
            var response = await Post("/api/v2/jobStatusUpdate", request);

            // Assert
            response.Should().Be(HttpStatusCode.Unauthorized);
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
            var workOrderId = await CreateWorkOrder();

            // Act
            await UpdateJob(workOrderId, req => req.TypeCode = updateCode);
            var heldOrder = GetWorkOrderFromDB(workOrderId);
            await UpdateJob(workOrderId, req =>
            {
                req.TypeCode = JobStatusUpdateTypeCode._0;
                req.OtherType = CustomJobStatusUpdates.Resume;
            });
            var resumedOrder = GetWorkOrderFromDB(workOrderId);

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

        public WorkOrder GetWorkOrderFromDB(int id)
        {
            using var ctx = GetContext();
            var db = ctx.DB;
            var repair = db.WorkOrders.Find(id);
            return repair;
        }

        public WorkOrder GetWorkOrderWithJobStatusUpdatesFromDB(int id)
        {
            using var ctx = GetContext();
            var db = ctx.DB;
            var repair = db.WorkOrders.Find(id);
            db.Entry(repair).Collection(r => r.JobStatusUpdates).Load();
            return repair;
        }

        public async Task<HttpStatusCode> CancelWorkOrder(int id)
        {
            var request = new Generator<WorkOrderComplete>()
                .AddWorkOrderCompleteGenerators()
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

        private async Task<HttpStatusCode> UpdateJob(int workOrderId, Action<JobStatusUpdate> interceptor = null)
        {
            var tasks = await GetTasks(workOrderId);
            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            JobStatusUpdate request = new Generator<JobStatusUpdate>()
                .AddJobStatusUpdateGenerators()
                .AddValue(JobStatusUpdateTypeCode._80, (JobStatusUpdate jsu) => jsu.TypeCode)
                .AddValue(workOrderId.ToString(), (JobStatusUpdate jsu) => jsu.RelatedWorkOrderReference.ID)
                .AddValue(workElement, (JobStatusUpdate jsu) => jsu.MoreSpecificSORCode)
                .AddValue("comments", (JobStatusUpdate jsu) => jsu.Comments)
                .Generate();

            interceptor?.Invoke(request);

            return await Post("/api/v2/jobStatusUpdate", request);
        }

        private async Task<int> CreateWorkOrder(Action<ScheduleRepair> interceptor = null)
        {
            var request = GenerateWorkOrder<ScheduleRepair>()
                .AddValue(new List<double> { 1 }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                .Generate();

            interceptor?.Invoke(request);

            var (code, response) = await Post<int>("/api/v2/workOrders/schedule", request);

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
