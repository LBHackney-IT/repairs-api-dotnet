using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RepairsApi.V2.Authorisation
{
    public class GroupOptions
    {
        public Dictionary<string, PermissionsModel> SecurityGroups { get; set; }
    }

    public static class SecurityGroup
    {
        public const string AGENT = "agent";
        public const string CONTRACTOR = "contractor";
    }

    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Generated struct from tuple")]
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Generated struct from tuple")]
    public class PermissionsModel
    {
        public string SecurityGroup;
        public string ContractorReference;

        public PermissionsModel()
        {

        }

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
