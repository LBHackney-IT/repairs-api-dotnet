using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure.Hackney;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Controllers
{
    public class ScheduleOfRatesControllerTests : ControllerTests
    {
        private ScheduleOfRatesController _classUnderTest;
        private Mock<IListScheduleOfRatesUseCase> _listScheduleOfRatesMock;
        private Mock<IListSorTradesUseCase> _listSorTradesMock;
        private Mock<ISorPriorityGateway> _priorityGatewayMock;
        private Mock<IScheduleOfRatesGateway> _scheduleOfRatesGatewayMock;

        [SetUp]
        public void Setup()
        {
            _listScheduleOfRatesMock = new Mock<IListScheduleOfRatesUseCase>();
            _listSorTradesMock = new Mock<IListSorTradesUseCase>();
            _priorityGatewayMock = new Mock<ISorPriorityGateway>();
            _scheduleOfRatesGatewayMock = new Mock<IScheduleOfRatesGateway>();
            _classUnderTest = new ScheduleOfRatesController(_listScheduleOfRatesMock.Object, _listSorTradesMock.Object, _priorityGatewayMock.Object, _scheduleOfRatesGatewayMock.Object);
        }

        [Test]
        public async Task ListSorCodes()
        {
            var model = new Generator<ScheduleOfRatesModel>().AddDefaultGenerators().GenerateList(10);

            const string TradeFilter = "trade";
            const string PropertyFilter = "prop";
            const string ContractorFilter = "contractor";

            _listScheduleOfRatesMock.Setup(m => m.Execute(TradeFilter, PropertyFilter, ContractorFilter)).ReturnsAsync(model);
            var result = await _classUnderTest.ListSorCodes(TradeFilter, PropertyFilter, ContractorFilter);

            var code = GetStatusCode(result);
            var response = GetResultData<IEnumerable<ScheduleOfRatesModel>>(result);

            code.Should().Be(200);
            response.Should().BeEquivalentTo(model);
        }

        [Test]
        public async Task ListTrades()
        {
            var model = new Generator<SorTradeResponse>().AddDefaultGenerators().GenerateList(10);

            const string PropertyFilter = "prop";

            _listSorTradesMock.Setup(m => m.Execute(PropertyFilter)).ReturnsAsync(model);
            var result = await _classUnderTest.ListTrades(PropertyFilter);

            var code = GetStatusCode(result);
            var response = GetResultData<IEnumerable<SorTradeResponse>>(result);

            code.Should().Be(200);
            response.Should().BeEquivalentTo(model);
        }

        [Test]
        public async Task ListPriorities()
        {
            var model = new Generator<SORPriority>().AddDefaultGenerators().GenerateList(10);

            _priorityGatewayMock.Setup(m => m.GetPriorities()).ReturnsAsync(model);
            var result = await _classUnderTest.ListPriorities();

            var code = GetStatusCode(result);
            var response = GetResultData<IEnumerable<SORPriority>>(result);

            code.Should().Be(200);
            response.Should().BeEquivalentTo(model);
        }

        [Test]
        public async Task GetSorCodeDefaultContractor()
        {
            const string SORCode = "sorCode";
            const string PropertyFilter = "prop";
            const string Contractor = "contractor";
            SetContractor(Contractor);
            var model = new Generator<ScheduleOfRatesModel>().AddDefaultGenerators().Generate();

            _scheduleOfRatesGatewayMock.Setup(m => m.GetCode(SORCode, PropertyFilter, Contractor)).ReturnsAsync(model);
            var result = await _classUnderTest.GetSorCode(SORCode, PropertyFilter);

            var code = GetStatusCode(result);
            var response = GetResultData<ScheduleOfRatesModel>(result);

            code.Should().Be(200);
            response.Should().Be(model);
        }

        [Test]
        public async Task GetSorCodeSpecifyValidContractor()
        {
            const string SORCode = "sorCode";
            const string PropertyFilter = "prop";
            const string Contractor = "contractor";
            const string OtherContractor = "otherContractor";
            SetContractor(Contractor, OtherContractor);
            var model = new Generator<ScheduleOfRatesModel>().AddDefaultGenerators().Generate();

            _scheduleOfRatesGatewayMock.Setup(m => m.GetCode(SORCode, PropertyFilter, Contractor)).ReturnsAsync(model);
            var result = await _classUnderTest.GetSorCode(SORCode, PropertyFilter, Contractor);

            var code = GetStatusCode(result);
            var response = GetResultData<ScheduleOfRatesModel>(result);

            code.Should().Be(200);
            response.Should().Be(model);
        }

        [Test]
        public async Task GetSorCodeSpecifyInValidContractor()
        {
            const string SORCode = "sorCode";
            const string PropertyFilter = "prop";
            const string Contractor = "contractor";
            const string OtherContractor = "otherContractor";
            const string InvalidContractor = "invalidContractor";
            SetContractor(Contractor, OtherContractor);
            var model = new Generator<ScheduleOfRatesModel>().AddDefaultGenerators().Generate();

            _scheduleOfRatesGatewayMock.Setup(m => m.GetCode(SORCode, PropertyFilter, Contractor)).ReturnsAsync(model);
            Func<Task> testFn = () => _classUnderTest.GetSorCode(SORCode, PropertyFilter, InvalidContractor);

            await testFn.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        private void SetContractor(params string[] contractors)
        {
            _classUnderTest.SetUser(new ClaimsPrincipal(new ClaimsIdentity(contractors.Select(c => new Claim(CustomClaimTypes.Contractor, c)))));
        }
    }
}
