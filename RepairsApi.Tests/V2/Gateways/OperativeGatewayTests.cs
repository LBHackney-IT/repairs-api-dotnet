using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Request;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.V2.Gateways
{
    [TestFixture]
    public class OperativeGatewayTests
    {
        private FilterBuilder<OperativeRequest, Operative> _filterBuilder;
        private readonly Fixture _fixture = new Fixture();
        private OperativeGateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _filterBuilder = new FilterBuilder<OperativeRequest, Operative>();
            _fixture.Customize<Operative>(c => c.Without(operative => operative.WorkElement));
            _classUnderTest = new OperativeGateway(InMemoryDb.Instance);
        }

        [TearDown]
        public void Teardown() => InMemoryDb.Teardown();

        [TestCase(0)]
        [TestCase(5)]
        public async Task RetrievesOperatives(int count)
        {
            // Arrange
            var operatives = _fixture.CreateMany<Operative>(count);
            var operativesSearchParams = new OperativeRequest();
            var filter = _filterBuilder.BuildFilter(operativesSearchParams);
            await InMemoryDb.Instance.Operatives.AddRangeAsync(operatives);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var dbResult = await _classUnderTest.GetByFilterAsync(filter);

            // Assert
            dbResult.Should().BeEquivalentTo(operatives);
        }

        [Test]
        public async Task RetrievesSingleOperative()
        {
            // Arrange
            var operative = _fixture.Create<Operative>();
            var operativePrn = operative.PayrollNumber;
            await InMemoryDb.Instance.Operatives.AddAsync(operative);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var dbResult = await _classUnderTest.GetByPayrollNumberAsync(operativePrn);

            // Assert
            dbResult.Should().BeEquivalentTo(operative);
        }
    }
}
