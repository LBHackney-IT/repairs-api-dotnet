using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Infrastructure;
using System.Linq;
using RepairsApi.V2;
using System;

namespace RepairsApi.Tests.V2.Factories
{
    public class ResponseFactoryTests
    {
        [Test]
        public void WorkOrderResponseMaps()
        {
            var workOrder = new Generator<WorkOrder>()
                .AddDefaultGenerators()
                .Generate();

            AppointmentDetails appointment = new AppointmentDetails
            {
                Date = DateTime.UtcNow,
                Description = "test",
                End = DateTime.UtcNow,
                Start = DateTime.UtcNow
            };
            var response = workOrder.ToResponse(appointment);

            PropertyClass propertyClass = workOrder.Site?.PropertyClass.FirstOrDefault();
            string addressLine = propertyClass?.Address?.AddressLine;
            response.CallerNumber.Should().Be(workOrder.Customer.Person.Communication.Where(cc => cc.Channel.Medium == RepairsApi.V2.Generated.CommunicationMediumCode._20).FirstOrDefault()?.Value);
            response.CallerName.Should().Be(workOrder.Customer.Person.Name.Full);
            response.DateRaised.Should().Be(workOrder.DateRaised);
            response.Description.Should().Be(workOrder.DescriptionOfWork);
            response.Owner.Should().Be(workOrder.AssignedToPrimary.Name);
            response.Priority.Should().Be(workOrder.WorkPriority.PriorityDescription);
            response.PriorityCode.Should().Be(workOrder.WorkPriority.PriorityCode);
            response.Property.Should().Be(addressLine);
            response.PropertyReference.Should().Be(workOrder.Site?.PropertyClass.FirstOrDefault()?.PropertyReference);
            response.Reference.Should().Be(workOrder.Id);
            response.Target.Should().Be(workOrder.WorkPriority.RequiredCompletionDateTime);
            response.Property.Should().Be(addressLine);
        }
    }
}
