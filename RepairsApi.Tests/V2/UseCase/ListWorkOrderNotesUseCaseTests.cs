using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    public class ListWorkOrderNotesUseCaseTests
    {
        private ListWorkOrderNotesUseCase _classUnderTest;
        private Mock<IRepairsGateway> _repairsGatewayMock;
        private Random _gen;

        [SetUp]
        public void Setup()
        {
            _gen = new Random();
            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _classUnderTest = new ListWorkOrderNotesUseCase(_repairsGatewayMock.Object);
        }

        [Test]
        public async Task CanListNotes()
        {
            var expected = new Generator<WorkOrder>()
                .AddDefaultGenerators()
                .AddGenerator(() => RandomDay(), (JobStatusUpdate jsu) => jsu.EventTime)
                .Generate();
            _repairsGatewayMock.Setup(m => m.GetWorkOrder(It.IsAny<int>())).ReturnsAsync(expected);

            var result = await _classUnderTest.Execute(expected.Id);

            result.Should().HaveCount(expected.JobStatusUpdates.Count(jsu => !jsu.Comments.IsNullOrEmpty()));
            result.AssertForEach(expected.JobStatusUpdates.OrderBy(jsu => jsu.EventTime),
                (nli, jsu) =>
                {
                    nli.Time.Should().Be(jsu.EventTime.Value);
                    nli.Note.Should().Be(jsu.Comments);
                    nli.User.Should().Be(jsu.Author);
                });
        }

        [Test]
        public async Task ThrowWhenNotFound()
        {
            _repairsGatewayMock.Setup(m => m.GetWorkOrder(It.IsAny<int>())).ReturnsAsync((WorkOrder) null);

            Func<Task> sutFunc = async () => await _classUnderTest.Execute(1);

            await sutFunc.Should().ThrowAsync<ResourceNotFoundException>();
        }

        private DateTime RandomDay()
        {
            var start = new DateTime(1995, 1, 1);
            var range = (DateTime.Today - start).Days;
            return start.AddDays(_gen.Next(range));
        }
    }
}
