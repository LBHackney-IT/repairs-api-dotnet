using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using NUnit.Framework;
using RepairsApi.Tests.V2.E2ETests;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Generated;

namespace RepairsApi.Tests.V2.Factories
{
    public class DBModelFactoryTests
    {
        [Test]
        public void ThrowsExceptionWhenMultipleAmountsProvided()
        {
            var quatity = new Quantity
            {
                Amount = new List<double>
                {
                    1.2, 5.6, 14.58
                }
            };

            Assert.Throws<NotSupportedException>(() => quatity.ToDb())
                .Message.Should().Be("Multiple amounts is not supported");
        }

        [Test]
        public void RaiseRepairMaps()
        {
            var request = RepairMockBuilder.CreateFullRaiseRepair();

            var db = request.ToDb();

            db.AccessInformation.Description.Should().Be(request.AccessInformation.Description);
            db.AccessInformation.Keysafe.Code.Should().Be(request.AccessInformation.Keysafe.Code);
            db.AccessInformation.Keysafe.Location.Should().Be(request.AccessInformation.Keysafe.Location);
            db.DateReported.Should().Be(request.DateReported);
            db.DescriptionOfWork.Should().Be(request.DescriptionOfWork);
            db.EstimatedLaborHours.Should().Be(request.EstimatedLaborHours);
            db.LocationOfRepair.Should().Be(request.LocationOfRepair);
            db.ParkingArrangements.Should().Be(request.ParkingArrangements);
            db.WorkType.Should().Be(request.WorkType);
            db.WorkPriority.RequiredCompletionDateTime.Should().Be(request.Priority.RequiredCompletionDateTime);
            db.WorkPriority.PriorityDescription.Should().Be(request.Priority.PriorityDescription);
            db.WorkPriority.PriorityCode.Should().Be(request.Priority.PriorityCode);
            db.WorkPriority.NumberOfDays.Should().Be(request.Priority.NumberOfDays);
            db.WorkPriority.Comments.Should().Be(request.Priority.Comments);
            db.WorkClass.WorkClassCode.Should().Be(request.WorkClass.WorkClassCode);
            db.WorkClass.WorkClassDescription.Should().Be(request.WorkClass.WorkClassDescription);
            db.WorkClass.WorkClassSubType.WorkClassSubTypeDescription.Should().Be(request.WorkClass.WorkClassSubType.WorkClassSubTypeDescription);
            db.WorkClass.WorkClassSubType.WorkClassSubTypeName.Should().Be(string.Join(',', request.WorkClass.WorkClassSubType.WorkClassSubType1));
        }
    }
}
