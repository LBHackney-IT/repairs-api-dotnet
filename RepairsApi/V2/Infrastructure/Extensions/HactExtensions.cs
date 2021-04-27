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
            return commList.FirstOrDefault(c => c.Channel?.Medium == CommunicationMediumCode._20)?.Value;
        }
    }
}
