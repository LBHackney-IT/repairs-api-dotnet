using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.UseCase;
using System;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    public class GetFilterUseCaseTests
    {
        private Mock<IActivatorWrapper<IFilterProvider>> _mockActivatorWrapper;
        private GetFilterUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _mockActivatorWrapper = new Mock<IActivatorWrapper<IFilterProvider>>();
            _classUnderTest = new GetFilterUseCase(_mockActivatorWrapper.Object);
        }

        [Test]
        public async Task ThrowsExceptionForUnknownModel()
        {
            Func<Task> testFn = () => _classUnderTest.Execute("unknown-model");

            await testFn.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test]
        public async Task CreatesWorkOrderFilter()
        {
            await VerifyFilterResolution<WorkOrderFilterProvider>(FilterConstants.WorkOrder);
        }

        public async Task VerifyFilterResolution<T>(string key)
            where T : IFilterProvider
        {
            _mockActivatorWrapper.Setup(m => m.CreateInstance<T>()).Returns(new Mock<IFilterProvider>().Object);
            await _classUnderTest.Execute(key);

            _mockActivatorWrapper.Verify(m => m.CreateInstance<T>());
        }
    }
}
