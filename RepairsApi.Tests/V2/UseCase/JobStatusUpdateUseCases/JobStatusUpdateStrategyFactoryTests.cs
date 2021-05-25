using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using JobStatusUpdate = RepairsApi.V2.Infrastructure.JobStatusUpdate;

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

        [GenericTestCase(typeof(AssignOperativesUseCase), JobStatusUpdateTypeCode._10)]
        [GenericTestCase(typeof(MoreSpecificSorUseCase), JobStatusUpdateTypeCode._80)]
        [GenericTestCase(typeof(ApproveVariationUseCase), JobStatusUpdateTypeCode._10020)]
        [GenericTestCase(typeof(RejectVariationUseCase), JobStatusUpdateTypeCode._125)]
        [GenericTestCase(typeof(ContractorAcknowledgeVariationUseCase), JobStatusUpdateTypeCode._10010)]
        [GenericTestCase(typeof(JobIncompleteStrategy), JobStatusUpdateTypeCode._120)]
        [GenericTestCase(typeof(JobIncompleteNeedMaterialsStrategy), JobStatusUpdateTypeCode._12020)]
        [GenericTestCase(typeof(RejectWorkOrderStrategy), JobStatusUpdateTypeCode._190)]
        [GenericTestCase(typeof(ApproveWorkOrderStrategy), JobStatusUpdateTypeCode._200)]
        [GenericTestCase(typeof(ResumeJobStrategy), JobStatusUpdateTypeCode._0, CustomJobStatusUpdates.Resume)]
        public async Task ValidateStrategyLookups<T>(JobStatusUpdateTypeCode typeCode, string otherTypeCode = "") where T : IJobStatusUpdateStrategy
        {
            await ValidateStrategyResolution<T>(new JobStatusUpdate
            {
                TypeCode = typeCode,
                OtherType = otherTypeCode
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

        [TestCase(JobStatusUpdateTypeCode._20, "")]
        [TestCase(JobStatusUpdateTypeCode._0, "notSupported")]
        public void ThrowUnsupportedWhenNonSupportedTypeCodes(JobStatusUpdateTypeCode typeCode, string otherTypeCode)
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.Id = 41;
            var jobStatusUpdate = new JobStatusUpdate
            {
                TypeCode = typeCode,
                OtherType = otherTypeCode
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
