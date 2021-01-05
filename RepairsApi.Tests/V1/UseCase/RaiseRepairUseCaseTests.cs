using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RepairsApi.V1.Domain.Repair;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V1.UseCase
{
    public class RaiseRepairUseCaseTests
    {
        private RaiseRepairUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IRepairsGateway>();
            _classUnderTest = new RaiseRepairUseCase(mock.Object, new NullLogger<RaiseRepairUseCase>());
        }

        [Test]
        public async Task Runs()
        {
            var result = await _classUnderTest.Execute(new WorkOrder());

            result.Should().BeTrue();
        }
    }
}
