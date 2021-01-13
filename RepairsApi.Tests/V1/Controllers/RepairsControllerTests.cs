using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V1.Controllers
{
    public class RepairsControllerTests : ControllerTests
    {
        private RepairsController _classUnderTest;
        private Mock<IRaiseRepairUseCase> _raiseRepairUseCaseMock;

        [SetUp]
        public void SetUp()
        {
            _raiseRepairUseCaseMock = new Mock<IRaiseRepairUseCase>();
            _classUnderTest = new RepairsController(_raiseRepairUseCaseMock.Object);
        }

        [Test]
        public async Task ReturnsOkWithInt()
        {
            int newId = 2;
            _raiseRepairUseCaseMock.Setup(m => m.Execute(It.IsAny<WorkOrder>())).ReturnsAsync(newId);
            var result = await _classUnderTest.RaiseRepair(new RaiseRepair());

            result.Should().BeOfType<OkObjectResult>();
            GetResultData<int>(result).Should().Be(newId);
        }
    }
}
