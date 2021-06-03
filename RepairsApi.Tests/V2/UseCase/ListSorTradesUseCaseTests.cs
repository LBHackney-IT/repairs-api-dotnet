using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure.Hackney;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    public class ListSorTradesUseCaseTests
    {
        private ListSorTradesUseCase _classUnderTest;
        private Mock<IScheduleOfRatesGateway> _sorGatewayMock;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Customize<SorCodeTrade>(c => c.Without(trade => trade.Operatives));
            _sorGatewayMock = new Mock<IScheduleOfRatesGateway>();
            _classUnderTest = new ListSorTradesUseCase(_sorGatewayMock.Object);
        }

        [Test]
        public async Task ReturnsTrades()
        {
            var expectedPropRef = _fixture.Create<string>();
            var sorCodeTrades = _fixture.CreateMany<SorCodeTrade>();
            _sorGatewayMock.Setup(x => x.GetTrades(expectedPropRef))
                .ReturnsAsync(sorCodeTrades);

            var trades = await _classUnderTest.Execute(expectedPropRef);

            var expectedTrades = sorCodeTrades.Select(t => t.ToResponse());
            trades.Should().BeEquivalentTo(expectedTrades);
        }
    }
}
