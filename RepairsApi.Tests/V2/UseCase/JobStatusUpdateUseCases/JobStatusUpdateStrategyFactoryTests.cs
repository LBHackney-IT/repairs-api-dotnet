using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class JobStatusUpdateStrategyFactoryTests
    {
        private Mock<IActivatorWrapper<IJobStatusUpdateStrategy>> _mockActivatorWrapper;
        private JobStatusUpdateStrategyFactory _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _mockActivatorWrapper = new Mock<IActivatorWrapper<IJobStatusUpdateStrategy>>();
            _classUnderTest = new JobStatusUpdateStrategyFactory(_mockActivatorWrapper.Object);
        }

        [Test]
        public async Task ExecutesMoreSpecificSORUseCase()
        {
            await ValidateStrategyResolution<MoreSpecificSorUseCase>(new JobStatusUpdate
            {
                TypeCode = JobStatusUpdateTypeCode._80
            });
        }

        [Test]
        public async Task ExecutesApproveVariationUseCase()
        {
            await ValidateStrategyResolution<ApproveVariationUseCase>(new JobStatusUpdate
            {
                TypeCode = JobStatusUpdateTypeCode._10020
            });
        }

        [Test]
        public async Task ExecutesRejectVariationUseCase()
        {
            await ValidateStrategyResolution<RejectVariationUseCase>(new JobStatusUpdate
            {
                TypeCode = JobStatusUpdateTypeCode._125
            });
        }

        [Test]
        public async Task ExecutesContractorAcceptApprovedVariationUseCase()
        {
            await ValidateStrategyResolution<ContractorAcknowledgeVariationUseCase>(new JobStatusUpdate
            {
                TypeCode = JobStatusUpdateTypeCode._10010
            });
        }

        [Test]
        public async Task JobIncomplete()
        {
            await ValidateStrategyResolution<JobIncompleteStrategy>(new JobStatusUpdate
            {
                TypeCode = JobStatusUpdateTypeCode._120
            });
        }

        [Test]
        public async Task ResumeJob()
        {
            await ValidateStrategyResolution<ResumeJobStrategy>(new JobStatusUpdate
            {
                TypeCode = JobStatusUpdateTypeCode._0,
                OtherType = CustomJobStatusUpdates.Resume
            });
        }

        private async Task ValidateStrategyResolution<T>(JobStatusUpdate jobStatusUpdate)
            where T : IJobStatusUpdateStrategy
        {
            var useCaseMock = new Mock<IJobStatusUpdateStrategy>();
            _mockActivatorWrapper.Setup(x => x.CreateInstance<T>())
                .Returns(useCaseMock.Object);

            await _classUnderTest.ProcessActions(jobStatusUpdate);

            useCaseMock.Verify(x => x.Execute(It.IsAny<JobStatusUpdate>()));
        }

        [Test]
        public void ThrowUnsupportedWhenNonSupportedTypeCodes()
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.Id = 41;
            var jobStatusUpdate = new JobStatusUpdate
            {
                TypeCode = JobStatusUpdateTypeCode._10
            };

            Assert.ThrowsAsync<NotSupportedException>(async () => await _classUnderTest.ProcessActions(jobStatusUpdate));
        }

        [Test]
        public void ThrowUnsupportedWhenNoTypeCode()
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.Id = 41;
            var jobStatusUpdate = new JobStatusUpdate();

            Assert.ThrowsAsync<NotSupportedException>(async () => await _classUnderTest.ProcessActions(jobStatusUpdate));
        }
    }
}
