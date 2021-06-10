using System;
using System.Collections.Generic;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Generated;

namespace RepairsApi.Tests.E2ETests.Repairs
{
    public static class WorkOrderHelpers
    {
        public static Generator<T> CreateWorkOrderGenerator<T>()
        {
            var gen = new Generator<T>()
                .AddWorkOrderGenerators()
                .AddValue(new List<double>
                {
                    new Random().Next(100)
                }, (RateScheduleItem rsi) => rsi.Quantity.Amount);

            return gen;
        }
    }
}
