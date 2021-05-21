using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;
using FluentAssertions;
using Microsoft.OpenApi.Validations.Rules;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.V2.Gateways
{
    [TestFixture]
    public class OperativeGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private OperativeGateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
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
            var operativesSearchParams = new RepairsApi.V2.Boundary.Request.OperativeRequest();
            await InMemoryDb.Instance.Operatives.AddRangeAsync(operatives);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var dbResult = await _classUnderTest.GetByQueryAsync(operativesSearchParams);

            // Assert
            dbResult.Should().BeEquivalentTo(operatives);
        }
    }
}
