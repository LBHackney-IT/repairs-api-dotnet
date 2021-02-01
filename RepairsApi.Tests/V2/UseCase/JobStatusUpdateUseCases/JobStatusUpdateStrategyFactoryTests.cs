using System;
using AutoFixture;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Generated;
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
        public void ExecutesMoreSpecificSORUseCase()
        {
            var useCaseMock = new Mock<IJobStatusUpdateStrategy>();
            _mockActivatorWrapper.Setup(x => x.CreateInstance<MoreSpecificSorUseCase>())
                .Returns(useCaseMock.Object);

            _classUnderTest.ProcessActions(new JobStatusUpdate
            {
                TypeCode = JobStatusUpdateTypeCode._80
            });

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
