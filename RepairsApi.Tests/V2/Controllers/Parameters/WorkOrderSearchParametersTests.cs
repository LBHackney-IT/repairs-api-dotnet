using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Controllers;

namespace RepairsApi.Tests.V2.Controllers.Parameters
{
    public class WorkOrderSearchParametersTests
    {
        private WorkOrderSearchParameters _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new WorkOrderSearchParameters();
        }
        [Test]
        public void ClampsPageSizeToMin0()
        {
            _classUnderTest.PageSize = -10;
            _classUnderTest.PageSize.Should().Be(0);
        }

        [Test]
        public void ClampsPageSizeToMax()
        {
            _classUnderTest.PageSize = WorkOrderSearchParameters.MaxPageSize + 10;
            _classUnderTest.PageSize.Should().Be(WorkOrderSearchParameters.MaxPageSize);
        }
    }
}
