using Bogus;
using RepairsApi.V2.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace RepairsApi.Tests.Helpers
{
    [SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "Test data")]
    public static class TestSorSeeder
    {
        public static string[] SORCodes =
        {
            "code1"
        };

        public static string[] ContractorCodes =
        {
            "contractor_code1"
        };

        public static string[] TradeCodes =
        {
            "trade_code1"
        };

        public static async Task SeedSorData(this RepairsContext context)
        {
            var generator = new Faker<ScheduleOfRates>();

            context.SORCodes.Add(new ScheduleOfRates
            {
                CustomCode = "code",
                CustomName = "name",
            });

            await context.SaveChangesAsync();
        }
    }
}
