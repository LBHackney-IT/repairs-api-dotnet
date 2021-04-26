using Moq;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    public class UpdateSorCodeUseCaseTests
    {
        private Mock<IScheduleOfRatesGateway> _sheduleOfRatesGateway;
        private UpdateSorCodesUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _sheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            _classUnderTest = new UpdateSorCodesUseCase(_sheduleOfRatesGateway.Object);
        }

        //[Test]
        //public async Task MoreSpecificSORCodeAddsSORCodeToWorkOrder()
        //{
        //    const int cost = 10;
        //    const int desiredWorkOrderId = 42;
        //    var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
        //    _updateSorCodesUseCaseMock.Setup(s => s.GetCost(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(cost);
        //    var expectedNewCodes = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem).ToList();
        //    expectedNewCodes.Add(new RateScheduleItem
        //    {
        //        CustomCode = "code",
        //        Quantity = new Quantity
        //        {
        //            Amount = 4.5
        //        }
        //    });
        //    var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCodes.ToArray());

        //    await _classUnderTest.Execute(request);

        //    List<RateScheduleItem> rateScheduleItems = workOrder.WorkElements.Single().RateScheduleItem;
        //    rateScheduleItems.Should().BeEquivalentTo(expectedNewCodes,
        //        options => options.Excluding(rsi => rsi.Id).Excluding(rsi => rsi.DateCreated).Excluding(rsi => rsi.CodeCost)
        //        .Excluding(rsi => rsi.OriginalId));
        //    rateScheduleItems.Last().CodeCost.Should().Be(cost);
        //}

        //[Test]
        //public async Task UpdateQuantityOfExistingCodes()
        //{
        //    const int desiredWorkOrderId = 42;
        //    var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
        //    var codeToModify = workOrder.WorkElements.First().RateScheduleItem.First();
        //    var expectedNewCode = CloneRateScheduleItem(codeToModify);
        //    expectedNewCode.Quantity.Amount += 4;
        //    var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCode);

        //    await _classUnderTest.Execute(request);

        //    codeToModify.Should().BeEquivalentTo(expectedNewCode,
        //        option => option.Excluding(x => x.Id).Excluding(x => x.Original)
        //        .Excluding(x => x.OriginalQuantity).Excluding(x => x.OriginalId));
        //}

        //[Test]
        //public void ThrowUnsupportedWhenOriginalSorCodeNotPResent()
        //{
        //    const int desiredWorkOrderId = 42;
        //    var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
        //    var expectedNewCode = new RateScheduleItem
        //    {
        //        CustomCode = "code",
        //        Quantity = new Quantity
        //        {
        //            Amount = 4.5
        //        }
        //    };
        //    var request = CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, expectedNewCode);

        //    Assert.ThrowsAsync<NotSupportedException>(() => _classUnderTest.Execute(request));
        //}

        //private static RateScheduleItem CloneRateScheduleItem(RateScheduleItem toModify)
        //{

        //    var expectedNewCodes = new RateScheduleItem
        //    {
        //        CustomCode = toModify.CustomCode,
        //        CustomName = toModify.CustomName,
        //        Quantity = new Quantity
        //        {
        //            Amount = toModify.Quantity.Amount,
        //            UnitOfMeasurementCode = toModify.Quantity.UnitOfMeasurementCode
        //        },
        //        CodeCost = toModify.CodeCost,
        //        DateCreated = toModify.DateCreated,
        //        M3NHFSORCode = toModify.M3NHFSORCode,
        //        Id = toModify.Id
        //    };
        //    return expectedNewCodes;
        //}



        //private WorkOrder CreateReturnWorkOrder(int expectedId)
        //{
        //    var workOrder = BuildWorkOrder(expectedId);

        //    _repairsGatewayMock.Setup(gateway => gateway.GetWorkOrder(It.Is<int>(i => i == expectedId)))
        //        .ReturnsAsync(workOrder);
        //    _repairsGatewayMock.Setup(gateway => gateway.GetWorkElementsForWorkOrder(It.Is<WorkOrder>(wo => wo.Id == expectedId)))
        //        .ReturnsAsync(_fixture.Create<List<WorkElement>>);

        //    return workOrder;
        //}
    }
}
