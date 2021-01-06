using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RepairsApi.V1.Controllers;
using RepairsApi.V1.Domain.Repair;
using RepairsApi.V1.UseCase.Interfaces;
using System.Net;
using System.Threading.Tasks;
using RepairsApi.V1.Boundary;

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
        public async Task ReturnsOk()
        {
            UseCaseReturns(true);
            var result = await _classUnderTest.RaiseRepair(new RaiseRepairRequest());

            var code = GetStatusCode(result);

            code.Should().Be(200);
        }

        [Test]
        public async Task ReturnsBadRequestWhenUnsuccesful()
        {
            UseCaseReturns(false);
            var result = await _classUnderTest.RaiseRepair(new RaiseRepairRequest());

            var code = GetStatusCode(result);

            code.Should().Be((int) HttpStatusCode.BadRequest);
        }

        private void UseCaseReturns(bool result)
        {
            _raiseRepairUseCaseMock.Setup(m => m.Execute(It.IsAny<WorkOrder>())).ReturnsAsync(result);
        }
    }
}
