using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;

namespace RepairsApi.Tests.V2.Controllers
{
    public class ContractorControllerTests : ControllerTests
    {

        private ContractorController _classUnderTest;
        private Mock<IScheduleOfRatesGateway> _contractorGateway;

        [SetUp]
        public void SetUp()
        {
            _contractorGateway = new Mock<IScheduleOfRatesGateway>();
            _classUnderTest = new ContractorController(_contractorGateway.Object);
        }

        [TestCase(0)]
        [TestCase(5)]
        public async Task ListsContractors(int contractorCount)
        {
            // Arrange
            var contractorGenerator = new Generator<Contractor>()
                .AddDefaultGenerators();
            var expectedContractors = contractorGenerator.GenerateList(contractorCount);

            _contractorGateway.Setup(x => x.GetContractors(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedContractors);

            // Act
            var response = await _classUnderTest.ListContractors(null, null);
            var contractors = GetResultData<List<Contractor>>(response);
            var statusCode = GetStatusCode(response);

            // Assert
            statusCode.Should().Be(200);
            contractors.Should().HaveCount(contractorCount);
        }
    }
}
