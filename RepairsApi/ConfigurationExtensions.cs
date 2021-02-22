using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace RepairsApi
{
    public static class ConfigurationExtensions
    {
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Handled by builder")]
        public static void AddGroups(this IConfigurationBuilder builder)
        {
            var json = Environment.GetEnvironmentVariable("GROUPS");

            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            builder.AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(json)));
        }
    }
}
