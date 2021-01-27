using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V2.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder, DataImporter dataImporter)
        {
            var sorCodeData = dataImporter.LoadData<SORImportModel>("SORCodes.json");
            SeedPriorities(modelBuilder, sorCodeData);
            SeedSORs(modelBuilder, sorCodeData);
        }

        private static void SeedPriorities(ModelBuilder modelBuilder, SORImportModel data)
        {
            modelBuilder.Entity<SORPriority>().HasData(data.Priorities);
        }

        private static void SeedSORs(ModelBuilder modelBuilder, SORImportModel data)
        {
            modelBuilder.Entity<ScheduleOfRates>().HasData(data.Codes.Select(c => new ScheduleOfRates
            {
                CustomCode = c.CustomCode,
                CustomName = c.CustomName,
                SORContractorRef = c.SorContractor,
                PriorityId = c.PriorityCode
            }));
        }
    }

    public class DataImporter
    {
        private readonly string _rootPath;
        private JsonSerializerOptions _jsonOptions;

        public DataImporter(string rootPath)
        {
            _rootPath = rootPath;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public T LoadData<T>(string fileName)
        {
            var jsonString = GetJson(fileName);
            return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);
        }

        private string GetJson(string fileName)
        {
            var path = Path.Combine(_rootPath, fileName);
            return File.ReadAllText(path);
        }
    }

    public class SORImportModel
    {
        public IEnumerable<ScheduleOfRatesImportModel> Codes { get; set; } = null!;
        public IEnumerable<SORPriority> Priorities { get; set; } = null!;
    }

    public class ScheduleOfRatesImportModel
    {

        public string CustomCode { get; set; }
        public string CustomName { get; set; }
        public int PriorityCode { get; set; }

        public string SorContractor { get; set; }
        public string TradeCode { get; set; }
    }
}
