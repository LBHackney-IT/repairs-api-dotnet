using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using System.IO;
using System.Threading.Tasks;
using System;

namespace SchemaGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            //Run("RaiseRepair-M3SoR-v7").Wait();
            //Run("WorkOrderComplete-M3SoR-v7").Wait();
            //Run("JobStatusUpdate-M3SoR-v7").Wait();
            //Run("ScheduleRepair-M3SoR-v7").Wait();
            Run("RequestAppointment").Wait();
        }

        public static async Task Run(string file)
        {
            var json = await File.ReadAllTextAsync($"json/{file}.json");

            var replacedJson = json
                .Replace(@"""type"": ""#/defini", "\"$ref\": \"#/defini")
                .Replace(@"""type"": ""date""", "\"type\": \"string\", \"format\": \"date\"")
                .Replace(@"""type"": ""date-time""", "\"type\": \"string\", \"format\": \"date-time\"");

            JObject parsedFile = (JObject) JsonConvert.DeserializeObject(replacedJson);

            var data = parsedFile.Last;
            JObject properties = data.Last as JObject;
            parsedFile.Add("properties", properties.GetValue("properties"));
            parsedFile.Add("required", properties.GetValue("required"));
            data.Remove();

            var fixedFile = JsonConvert.SerializeObject(parsedFile);

            var schema = await JsonSchema.FromJsonAsync(fixedFile);
            schema.Title = file.Split('-', '.')[0];
            DisallowAdditionalProperties(schema, "");
            var generator = new CSharpGenerator(schema, new CSharpGeneratorSettings
            {
                DateTimeType = "System.DateTime",
                DateType = "System.DateTime",
                Namespace = "RepairsApi.V2.Generated",
                GenerateDefaultValues = false,
                GenerateOptionalPropertiesAsNullable = true
            });
            var codeFile = generator.GenerateFile();

            await File.WriteAllTextAsync($"../../../../RepairsApi/V2/Generated/{file}.cs", codeFile);
        }

        private static void DisallowAdditionalProperties(JsonSchema schema, string pref)
        {
            if (schema is null) return;

            schema.AllowAdditionalProperties = false;

            foreach (var kv in schema.Definitions)
            {
                string arrow = pref + " -> def -> " + kv.Key;
                Console.WriteLine(arrow);
                DisallowAdditionalProperties(kv.Value, arrow);
            }

            foreach (var kv in schema.Properties)
            {
                if (kv.Value.Name == "M3NHFSORCode")
                {
                    kv.Value.ActualTypeSchema.Enumeration.Clear();
                }
                string arrow = pref + " -> prop -> " + kv.Key;
                Console.WriteLine(arrow);
                DisallowAdditionalProperties(kv.Value, arrow);
            }


            DisallowAdditionalProperties(schema.Item, " -> item -> ");
        }
    }
}
