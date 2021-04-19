using System;
using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.Infrastructure.Extensions
{
    public static class HactExtensions
    {
        public static string GetPhoneNumber(this IEnumerable<Communication> commList)
        {
            return commList.FirstOrDefault(c => c.Channel.Medium == CommunicationMediumCode._20)?.Value;
        }

        public static string ToLegacyPriority(this WorkPriority priority) =>
            priority.PriorityCode switch
            {
                WorkPriorityCode._1 => "I",
                WorkPriorityCode._2 => "I",
                WorkPriorityCode._3 => "E",
                WorkPriorityCode._4 => "U",
                WorkPriorityCode._5 => "N",
                _ => throw new NotSupportedException(Resources.WorkPriorityCodeMissing)
            };

        public static WorkPriorityCode[] ToUhPriority(this string priority) =>
            priority switch
            {
                "I" => ToArray(WorkPriorityCode._1, WorkPriorityCode._2),
                "E" => ToArray(WorkPriorityCode._3),
                "U" => ToArray(WorkPriorityCode._4),
                "N" => ToArray(WorkPriorityCode._5),
                _ => throw new NotSupportedException(Resources.UnsupportedUHPriority)
            };

        public static IEnumerable<WorkPriorityCode> GetHactCodes(this IEnumerable<string> uhCodes)
        {
            List<WorkPriorityCode> result = new List<WorkPriorityCode>();

            foreach (var item in uhCodes)
            {
                result.AddRange(item.ToUhPriority());
            }

            return result;
        }

        private static T[] ToArray<T>(params T[] data)
        {
            return data;
        }
    }
}
