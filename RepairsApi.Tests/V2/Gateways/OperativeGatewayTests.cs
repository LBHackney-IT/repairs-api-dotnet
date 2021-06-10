using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Request;
using RepairsApi.V2.Exceptions;
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
        private OperativesGateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _filterBuilder = new FilterBuilder<OperativeRequest, Operative>();
            _fixture.Customize<Operative>(c => c
                .Without(operative => operative.AssignedWorkOrders)
                .Without(operative => operative.WorkOrderOperatives)
                .Without(operative => operative.Trades)
                .With(operative => operative.IsArchived, false));
            _classUnderTest = new OperativesGateway(InMemoryDb.Instance);
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
            var dbResult = await _classUnderTest.ListByFilterAsync(filter);

            // Assert
            dbResult.Should().BeEquivalentTo(operatives);
        }

        [Test]
        public async Task ListDoesNotRetrieveArchivedOperatives()
        {
            // Arrange
            const int count = 5;
            var index = new Random().Next(0, count - 1);
            var operatives = _fixture.CreateMany<Operative>(count);
            var operativesSearchParams = new OperativeRequest();
            var filter = _filterBuilder.BuildFilter(operativesSearchParams);
            await InMemoryDb.Instance.Operatives.AddRangeAsync(operatives);
            await InMemoryDb.Instance.SaveChangesAsync();
            await _classUnderTest.ArchiveAsync(operatives.ElementAt(index).PayrollNumber);

            // Act
            var dbResult = await _classUnderTest.ListByFilterAsync(filter);

            // Assert
            dbResult.Count().Should().Be(count - 1);
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
            var dbResult = await _classUnderTest.GetAsync(operativePrn);

            // Assert
            dbResult.Should().BeEquivalentTo(operative);
        }

        [Test]
        public async Task ArchivesOperativeAndReturnsTrue()
        {
            // Arrange
            const int count = 5;
            var index = new Random().Next(0, count - 1);
            var operatives = _fixture.CreateMany<Operative>(count);
            var operativePrn = operatives.ElementAt(index).PayrollNumber;
            await InMemoryDb.Instance.Operatives.AddRangeAsync(operatives);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            await _classUnderTest.ArchiveAsync(operativePrn);

            // Assert
            InMemoryDb.Instance.Operatives.Count().Should().Be(count - 1);
        }

        [Test]
        public async Task ThrowsIfNotFound()
        {
            // Arrange
            var operativePrn = _fixture.Create<Operative>().PayrollNumber;

            // Act
            Func<Task> testFn = () => _classUnderTest.ArchiveAsync(operativePrn);

            await testFn.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test]
        public async Task ArchivedOperativesAreNotDeleted()
        {
            // Arrange
            var operative = _fixture.Create<Operative>();
            var operativePrn = operative.PayrollNumber;
            await InMemoryDb.Instance.Operatives.AddAsync(operative);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            await _classUnderTest.ArchiveAsync(operativePrn);
            var archivedOperative = await _classUnderTest.GetAsync(operativePrn);

            // Assert
            archivedOperative.Should().NotBeNull("operatives are only SOFT-deleted");
        }

        [Test]
        public async Task AssignsOperatives()
        {
            var assignment = new WorkOrderOperative() { OperativeId = 1, WorkOrderId = 1 };
            await _classUnderTest.AssignOperatives(assignment);

            InMemoryDb.Instance.WorkOrderOperatives.Should().Contain(assignment);
        }
    }
}
