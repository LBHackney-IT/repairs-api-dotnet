using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RepairsApi.V2.Authorisation
{

    public interface IGroups
    {
        Dictionary<string, PermissionsModel> SecurityGroups { get; }
        Dictionary<string, double> RaiseLimitGroups { get; }
        Dictionary<string, double> VaryLimitGroups { get; }
    }

    public class Groups : IGroups
    {
        public Dictionary<string, PermissionsModel> SecurityGroups { get; } = new Dictionary<string, PermissionsModel>
        {
            { "repairs-hub-frontend-staging", (SecurityGroup.AGENT, null) },
            { "repairs-hub-frontend-staging-contractors-alphatrack", (SecurityGroup.CONTRACTOR, "ASL") },
            { "repairs-hub-frontend-staging-contractors-purdy", (SecurityGroup.CONTRACTOR, "PCL") },
        };

        public Dictionary<string, double> RaiseLimitGroups { get; } = new Dictionary<string, double>
        {
            { "repairs-hub-frontend-staging-raiselimit50", 50 },
            { "repairs-hub-frontend-staging-raiselimit150", 150 },
        };

        public Dictionary<string, double> VaryLimitGroups { get; } = new Dictionary<string, double>
        {
            { "repairs-hub-frontend-staging-varylimit50", 50 },
            { "repairs-hub-frontend-staging-varylimit150", 150 },
        };
    }

    public static class SecurityGroup
    {
        public const string AGENT = "agent";
        public const string CONTRACTOR = "contractor";
    }

    [SuppressMessage("Design", "CA1066:Type {0} should implement IEquatable<T> because it overrides Equals", Justification = "Generated struct from tuple")]
    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Generated struct from tuple")]
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Generated struct from tuple")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Generated struct from tuple")]
    [SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "Generated struct from tuple")]
    public struct PermissionsModel
    {
        public string SecurityGroup;
        public string ContractorReference;

        public PermissionsModel(string securityGroup, string contractorReference)
        {
            SecurityGroup = securityGroup;
            ContractorReference = contractorReference;
        }

        public override bool Equals(object obj)
        {
            return obj is PermissionsModel other &&
                   SecurityGroup == other.SecurityGroup &&
                   ContractorReference == other.ContractorReference;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SecurityGroup, ContractorReference);
        }

        public void Deconstruct(out string securityGroup, out string contractorReference)
        {
            securityGroup = SecurityGroup;
            contractorReference = ContractorReference;
        }

        public static implicit operator (string, string)(PermissionsModel value)
        {
            return (value.SecurityGroup, value.ContractorReference);
        }

        public static implicit operator PermissionsModel((string SecurityGroup, string ContractorReference) value)
        {
            return new PermissionsModel(value.SecurityGroup, value.ContractorReference);
        }
    }
}
