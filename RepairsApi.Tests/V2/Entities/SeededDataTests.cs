using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace RepairsApi.Tests.V2.Entities
{
    public class SeededDataTests
    {
        [Test]
        public async Task SeededDataExists()
        {
            await PrioritiesSeeded();
            await SorCodesSeeded();
        }

        private static async Task PrioritiesSeeded()
        {

            var data = await InMemoryDb.Instance.SORPriorities.ToListAsync();
            data.Should().NotBeNullOrEmpty();
        }

        private static async Task SorCodesSeeded()
        {

            var data = await InMemoryDb.Instance.SORCodes.ToListAsync();
            data.Should().NotBeNullOrEmpty();
        }
    }
}
