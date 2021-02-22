using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RepairsApi.V2.Authorisation
{
    public class GroupOptions
    {
        public Dictionary<string, PermissionsModel> SecurityGroups { get; set; }
        public Dictionary<string, SpendLimitModel> RaiseLimitGroups { get; set; }
        public Dictionary<string, SpendLimitModel> VaryLimitGroups { get; set; }

        public override string ToString()
        {
            return $"{SecurityGroups.Count} Security Groups";
        }
    }

    public static class SecurityGroup
    {
        public const string AGENT = "agent";
        public const string CONTRACTOR = "contractor";
    }

    public class PermissionsModel
    {
        public string SecurityGroup { get; set; }
        public string ContractorReference { get; set; }
    }

    public class SpendLimitModel
    {
        public double Limit { get; set; }
    }
}
