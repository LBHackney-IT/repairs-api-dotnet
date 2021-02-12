using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Gateways
{
    public class AppointmentGatewayTests
    {
        private AppointmentGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new AppointmentGateway(InMemoryDb.Instance);
        }

        [Test]
        public async Task ListForToday()
        {
            SeedData("contractor", new DaySeedModel[]
            {
                    new DaySeedModel(DateTime.UtcNow.DayOfWeek, 5),
            }, new AppointmentSeedModel[]
            {
                    new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12)),
                    new AppointmentSeedModel("PM", new DateTime().AddHours(13), new DateTime().AddHours(18)),
            });

            var items = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            items.Should().HaveCount(2);

            items.Count(item => item.Description == "AM").Should().Be(1);
            items.Count(item => item.Description == "PM").Should().Be(1);
        }

        [Test]
        public async Task OnlyIncludeRelevantDays()
        {
            SeedData("contractor", new DaySeedModel[]
            {
                    new DaySeedModel(DateTime.UtcNow.DayOfWeek, 5),
                    new DaySeedModel(DateTime.UtcNow.AddDays(1).DayOfWeek, 5),
                    new DaySeedModel(DateTime.UtcNow.AddDays(2).DayOfWeek, 5),
            }, new AppointmentSeedModel[]
            {
                    new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12)),
                    new AppointmentSeedModel("PM", new DateTime().AddHours(13), new DateTime().AddHours(18)),
            });

            var items = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            items.Should().HaveCount(2);
            items.Count(item => item.Description == "AM").Should().Be(1);
            items.Count(item => item.Description == "PM").Should().Be(1);
        }

        [Test]
        public async Task EmptyListForNoAppointments()
        {
            SeedData("contractor", new DaySeedModel[]
            {
                    new DaySeedModel(DateTime.UtcNow.DayOfWeek, 0),
            }, new AppointmentSeedModel[]
            {
                    new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12)),
                    new AppointmentSeedModel("PM", new DateTime().AddHours(13), new DateTime().AddHours(18)),
            });

            var items = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            items.Should().BeEmpty();
        }

        [Test]
        public async Task EmptyListForOtherContractor()
        {
            SeedData("contractor", new DaySeedModel[]
            {
                    new DaySeedModel(DateTime.UtcNow.DayOfWeek, 5),
            }, new AppointmentSeedModel[]
            {
                    new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12)),
                    new AppointmentSeedModel("PM", new DateTime().AddHours(13), new DateTime().AddHours(18)),
            });

            var items = await _classUnderTest.ListAppointments("otherContractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            items.Should().BeEmpty();
        }

        [Test]
        public async Task ThrowsIfDatesAreWrongWayRound()
        {
            Func<Task> testFunc = async () => await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(8), DateTime.UtcNow.AddDays(1));

            await testFunc.Should().ThrowAsync<NotSupportedException>();
        }


        [Test]
        public async Task BookAppointment()
        {
            var ids = SeedData("contractor", new DaySeedModel[]
            {
                    new DaySeedModel(DateTime.UtcNow.DayOfWeek, 1),
            }, new AppointmentSeedModel[]
            {
                    new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12))
            });

            var preBookItems = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
            await _classUnderTest.Create(GenerateAppointmentRef(ids.First(), DateTime.UtcNow ), 100001);
            var postBookItems = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            preBookItems.Should().HaveCount(1);
            postBookItems.Should().HaveCount(0);
        }

        private static string GenerateAppointmentRef(int id, DateTime date)
        {
            return $"{id.ToString()}/{date:dd-MM-yyyy}";
        }

        [Test]
        public async Task ThrowsWhenOverBooking()
        {
            var ids = SeedData("contractor", new DaySeedModel[]
            {
                    new DaySeedModel(DateTime.UtcNow.DayOfWeek, 1),
            }, new AppointmentSeedModel[]
            {
                    new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12))
            });
            await _classUnderTest.Create(GenerateAppointmentRef(ids.First(), DateTime.UtcNow), 100001);

            Func<Task> testFunc = async () => await _classUnderTest.Create(GenerateAppointmentRef(ids.First(), DateTime.UtcNow), 100001);

            await testFunc.Should().ThrowAsync<NotSupportedException>();
        }


        [Test]
        public async Task ThrowsWhenNoMatchingAppointment()
        {
            Func<Task> testFunc = async () => await _classUnderTest.Create("1/01-01-2020", 100001);

            await testFunc.Should().ThrowAsync<ResourceNotFoundException>();
        }

        private static List<int> SeedData(string contractor, DaySeedModel[] days, AppointmentSeedModel[] appointments)
        {
            List<int> bookableAppointmentsIds = new List<int>();

            foreach (var app in appointments)
            {
                var entry = InMemoryDb.Instance.AvailableAppointments.Add(new RepairsApi.V2.Infrastructure.Hackney.AvailableAppointment
                {
                    ContractorReference = contractor,
                    Description = app.Description,
                    StartTime = app.StartTime,
                    EndTime = app.EndTime,
                });

                foreach (var day in days)
                {
                    var innerEntry = InMemoryDb.Instance.AvailableAppointmentDays.Add(new RepairsApi.V2.Infrastructure.Hackney.AvailableAppointmentDay
                    {
                        AvailableAppointmentId = entry.Entity.Id,
                        AvailableCount = day.Count,
                        Day = day.Day
                    });

                    bookableAppointmentsIds.Add(innerEntry.Entity.Id);
                }

                InMemoryDb.Instance.SaveChanges();
            }

            return bookableAppointmentsIds;
        }

        [TearDown]
        public void TearDown()
        {
            InMemoryDb.Teardown();
        }
    }

    internal class AppointmentSeedModel
    {
        public AppointmentSeedModel(string description, DateTime startTime, DateTime endTime)
        {
            Description = description;
            StartTime = startTime;
            EndTime = endTime;
        }

        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    internal class DaySeedModel
    {
        public DaySeedModel(DayOfWeek day, int count)
        {
            Day = day;
            Count = count;
        }

        public DayOfWeek Day { get; set; }
        public int Count { get; set; }
    }
}
