using Bogus;
using RepairsApi.V2.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RepairsApi.V2.Infrastructure;
using GeographicalLocation = RepairsApi.V2.Generated.GeographicalLocation;
using JobStatusUpdate = RepairsApi.V2.Infrastructure.JobStatusUpdate;
using Organization = RepairsApi.V2.Generated.Organization;
using PropertyAddress = RepairsApi.V2.Generated.PropertyAddress;
using Quantity = RepairsApi.V2.Generated.Quantity;
using RateScheduleItem = RepairsApi.V2.Generated.RateScheduleItem;
using Trade = RepairsApi.V2.Generated.Trade;
using WorkElement = RepairsApi.V2.Generated.WorkElement;

namespace RepairsApi.Tests.Helpers.StubGeneration
{
    public static class ObjectGenerationExtensions
    {
        public static Generator<T> AddDefaultGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddGenerator(new RandomStringGenerator(10))
                .AddGenerator(new RandomCharGenerator())
                .AddGenerator(new RandomDoubleGenerator(0, 50))
                .AddGenerator(new RandomDateGenerator(20))
                .AddGenerator(new RandomBoolGenerator());
        }

        public static Generator<T> AddValue<T, TValue>(this Generator<T> generator, TValue value)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TValue>(value));
        }

        public static Generator<T> AddGenerator<T, TValue>(this Generator<T> generator, Func<TValue> valueGenerator)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TValue>(valueGenerator));
        }

        public static Generator<T> AddValue<T, TModel, TProp>(this Generator<T> generator, TProp value, params Expression<Func<TModel, TProp>>[] accessors)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TProp>(value), accessors);
        }

        public static Generator<T> Ignore<T, TModel, TProp>(this Generator<T> generator, params Expression<Func<TModel, TProp>>[] accessors)
            where TProp : class
        {
            return generator.AddValue(null, accessors);
        }

        public static Generator<T> AddGenerator<T, TModel, TProp>(this Generator<T> generator, Func<TProp> valueGenerator, params Expression<Func<TModel, TProp>>[] accessors)
        {
            return generator
                .AddGenerator(new SimpleValueGenerator<TProp>(valueGenerator), accessors);
        }

        public static Generator<T> AddWorkOrderGenerators<T>(this Generator<T> generator)
        {
            var contractorReference = new List<Reference>()
                {
                    new Reference
                    {
                        ID = TestDataSeeder.Contractor
                    }
                };

            return generator
                .AddDefaultGenerators()
                .AddValue(new string[] { "address", "line" }, (PropertyAddress addr) => addr.AddressLine)
                .AddValue(new string[] { "address", "line" }, (Address addr) => addr.AddressLine)
                .AddValue(GetSitePropertyUnitGenerator(), (RaiseRepair rr) => rr.SitePropertyUnit)
                .AddValue("propref", (Property p) => p.PropertyReference)
                .AddValue(new List<double> { 2.0 }, (Quantity q) => q.Amount)
                .AddValue(contractorReference, (Organization o) => o.Reference)
                .AddValue(TestDataSeeder.SorCode, (RateScheduleItem rsi) => rsi.CustomCode)
                .AddValue(new string[] { "2.0" },
                    (GeographicalLocation q) => q.Latitude,
                    (GeographicalLocation q) => q.Longitude,
                    (GeographicalLocation q) => q.Elevation,
                    (GeographicalLocation q) => q.ElevationReferenceSystem)
                .AddValue("trade", (Trade t) => t.CustomCode)
                .SetListLength<WorkElement>(1)
                .SetListLength<Trade>(1)
                .SetListLength<RateScheduleItem>(1)
                .Ignore((WorkOrder wo) => wo.AssignedOperatives)
                .Ignore((WorkOrder wo) => wo.WorkOrderOperatives);
        }

        public static Generator<T> WithSorCodes<T>(this Generator<T> generator, params string[] sorCodes)
        {
            Random rand = new Random();
            var sorCode = sorCodes[rand.Next(sorCodes.Length)];

            return generator
                .AddGenerator(() => sorCode, (RateScheduleItem rsi) => rsi.CustomCode);
        }

        public static Generator<T> AddInfrastructureWorkOrderGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddDefaultGenerators()
                .AddValue(RepairsApi.V2.Infrastructure.WorkStatusCode.Open, (RepairsApi.V2.Infrastructure.WorkOrder wo) => wo.StatusCode)
                .AddValue(null, (RepairsApi.V2.Infrastructure.WorkOrder wo) => wo.WorkOrderComplete)
                .AddValue(null, (RepairsApi.V2.Infrastructure.WorkOrder wo) => wo.JobStatusUpdates)
                .AddValue(false, (RepairsApi.V2.Infrastructure.RateScheduleItem rsi) => rsi.Original)
                .AddValue(1, (RepairsApi.V2.Infrastructure.WorkOrder wo) => wo.WorkPriority.PriorityCode)
                .AddValue(DateTime.UtcNow, (RepairsApi.V2.Infrastructure.WorkOrder wo) => wo.WorkPriority.RequiredCompletionDateTime)
                .Ignore((WorkOrder wo) => wo.AssignedOperatives)
                .Ignore((WorkOrder wo) => wo.WorkOrderOperatives);
        }

        private static ICollection<SitePropertyUnit> GetSitePropertyUnitGenerator()
        {
            var generator = new Generator<SitePropertyUnit>()
                .AddDefaultGenerators()
                .AddValue(new string[] { "address", "line" }, (RepairsApi.V2.Generated.PropertyAddress addr) => addr.AddressLine)
                .AddValue(new string[] { "address", "line" }, (RepairsApi.V2.Generated.Address addr) => addr.AddressLine);
            return generator.GenerateList(1);
        }

        public static Generator<T> AddWorkOrderCompleteGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddWorkOrderGenerators()
                .AddValue(null, (RepairsApi.V2.Generated.WorkOrderComplete woc) => woc.FollowOnWorkOrderReference)
                .AddValue(null, (RepairsApi.V2.Generated.JobStatusUpdates jsu) => jsu.RelatedWorkElementReference)
                .AddValue(null, (RepairsApi.V2.Generated.JobStatusUpdates jsu) => jsu.AdditionalWork);
        }

        public static Generator<T> AddJobStatusUpdateGenerators<T>(this Generator<T> generator)
        {
            return generator
                .AddDefaultGenerators()
                .AddWorkOrderGenerators()
                .AddValue(null, (RepairsApi.V2.Generated.JobStatusUpdate jsu) => jsu.AdditionalWork)
                .SetListLength<WorkElement>(1)
                .SetListLength<RateScheduleItem>(1);
        }
    }
}
