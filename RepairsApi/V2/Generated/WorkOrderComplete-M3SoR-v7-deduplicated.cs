//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.3.2.0 (Newtonsoft.Json v9.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------

namespace RepairsApi.V2.Generated
{
    #pragma warning disable // Disable all warnings

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class GeographicalLocation
    {
        [Newtonsoft.Json.JsonProperty("Longitude", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> Longitude { get; set; }

        [Newtonsoft.Json.JsonProperty("Latitude", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> Latitude { get; set; }

        [Newtonsoft.Json.JsonProperty("Elevation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> Elevation { get; set; }

        [Newtonsoft.Json.JsonProperty("ElevationReferenceSystem", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> ElevationReferenceSystem { get; set; }

        [Newtonsoft.Json.JsonProperty("PositionalAccuracy", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string PositionalAccuracy { get; set; }

        [Newtonsoft.Json.JsonProperty("Polyline", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Polyline> Polyline { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class PropertyAddress
    {
        [Newtonsoft.Json.JsonProperty("Postbox", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Postbox { get; set; }

        [Newtonsoft.Json.JsonProperty("Room", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Room { get; set; }

        [Newtonsoft.Json.JsonProperty("Department", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Department { get; set; }

        [Newtonsoft.Json.JsonProperty("Floor", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Floor { get; set; }

        [Newtonsoft.Json.JsonProperty("Plot", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Plot { get; set; }

        [Newtonsoft.Json.JsonProperty("BuildingNumber", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string BuildingNumber { get; set; }

        [Newtonsoft.Json.JsonProperty("BuildingName", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string BuildingName { get; set; }

        [Newtonsoft.Json.JsonProperty("ComplexName", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ComplexName { get; set; }

        [Newtonsoft.Json.JsonProperty("StreetName", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string StreetName { get; set; }

        [Newtonsoft.Json.JsonProperty("CityName", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CityName { get; set; }

        [Newtonsoft.Json.JsonProperty("Country", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public CountryCode? Country { get; set; }

        [Newtonsoft.Json.JsonProperty("AddressLine", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.MaxLength(7)]
        public System.Collections.Generic.ICollection<string> AddressLine { get; set; }

        [Newtonsoft.Json.JsonProperty("Type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Type { get; set; }

        [Newtonsoft.Json.JsonProperty("PostalCode", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string PostalCode { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public enum CommunicationChannelCodeC0
    {
        [System.Runtime.Serialization.EnumMember(Value = @"10")]
        _10 = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"40")]
        _40 = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"60")]
        _60 = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"70")]
        _70 = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"70-10")]
        _7010 = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"70-20")]
        _7020 = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"70-30")]
        _7030 = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"70-40")]
        _7040 = 7,

        [System.Runtime.Serialization.EnumMember(Value = @"70-50")]
        _7050 = 8,

        [System.Runtime.Serialization.EnumMember(Value = @"70-60")]
        _7060 = 9,

        [System.Runtime.Serialization.EnumMember(Value = @"70-70")]
        _7070 = 10,

        [System.Runtime.Serialization.EnumMember(Value = @"70-80")]
        _7080 = 11,

        [System.Runtime.Serialization.EnumMember(Value = @"70-90")]
        _7090 = 12,

        [System.Runtime.Serialization.EnumMember(Value = @"70-100")]
        _70100 = 13,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public enum CommunicationMediumCode
    {
        [System.Runtime.Serialization.EnumMember(Value = @"0")]
        _0 = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"10")]
        _10 = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"10-10")]
        _1010 = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"10-20")]
        _1020 = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"20")]
        _20 = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"30")]
        _30 = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"30-10")]
        _3010 = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"30-20")]
        _3020 = 7,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Party
    {
        [Newtonsoft.Json.JsonProperty("Reference", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Reference> Reference { get; set; }

        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Role", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Role { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Identification
    {
        [Newtonsoft.Json.JsonProperty("Type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Type { get; set; }

        [Newtonsoft.Json.JsonProperty("Number", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Number { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public enum JobStatusUpdateTypeCode
    {
        [System.Runtime.Serialization.EnumMember(Value = @"0")]
        _0 = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"10")]
        _10 = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"20")]
        _20 = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"30")]
        _30 = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"40")]
        _40 = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"50")]
        _50 = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"60")]
        _60 = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"70")]
        _70 = 7,

        [System.Runtime.Serialization.EnumMember(Value = @"80")]
        _80 = 8,

        [System.Runtime.Serialization.EnumMember(Value = @"90")]
        _90 = 9,

        [System.Runtime.Serialization.EnumMember(Value = @"100")]
        _100 = 10,

        [System.Runtime.Serialization.EnumMember(Value = @"100-10")]
        _10010 = 11,

        [System.Runtime.Serialization.EnumMember(Value = @"100-20")]
        _10020 = 12,

        [System.Runtime.Serialization.EnumMember(Value = @"110")]
        _110 = 13,

        [System.Runtime.Serialization.EnumMember(Value = @"120")]
        _120 = 14,

        [System.Runtime.Serialization.EnumMember(Value = @"120-10")]
        _12010 = 15,

        [System.Runtime.Serialization.EnumMember(Value = @"120-20")]
        _12020 = 16,

        [System.Runtime.Serialization.EnumMember(Value = @"120-30")]
        _12030 = 17,

        [System.Runtime.Serialization.EnumMember(Value = @"120-40")]
        _12040 = 18,

        [System.Runtime.Serialization.EnumMember(Value = @"120-50")]
        _12050 = 19,

        [System.Runtime.Serialization.EnumMember(Value = @"125")]
        _125 = 20,

        //Variation attempted contract manager approval required
        [System.Runtime.Serialization.EnumMember(Value = @"180")]
        _180 = 21,
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class WorkOrderComplete
    {
        [Newtonsoft.Json.JsonProperty("WorkOrderReference", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public Reference WorkOrderReference { get; set; } = new Reference();

        [Newtonsoft.Json.JsonProperty("BillOfMaterialItem", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<RateScheduleItem> BillOfMaterialItem { get; set; }

        [Newtonsoft.Json.JsonProperty("CompletedWorkElements", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<WorkElement> CompletedWorkElements { get; set; }

        [Newtonsoft.Json.JsonProperty("OperativesUsed", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<OperativesUsed> OperativesUsed { get; set; }

        [Newtonsoft.Json.JsonProperty("JobStatusUpdates", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<JobStatusUpdates> JobStatusUpdates { get; set; }

        [Newtonsoft.Json.JsonProperty("FollowOnWorkOrderReference", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Reference> FollowOnWorkOrderReference { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Polyline
    {
        [Newtonsoft.Json.JsonProperty("Sequence", Required = Newtonsoft.Json.Required.Always)]
        public int Sequence { get; set; }

        [Newtonsoft.Json.JsonProperty("Easting", Required = Newtonsoft.Json.Required.Always)]
        public double Easting { get; set; }

        [Newtonsoft.Json.JsonProperty("Northing", Required = Newtonsoft.Json.Required.Always)]
        public double Northing { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class OperativesUsed
    {
        [Newtonsoft.Json.JsonProperty("Trade", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Trade> Trade { get; set; }

        [Newtonsoft.Json.JsonProperty("WorkElementReference", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Reference> WorkElementReference { get; set; }

        [Newtonsoft.Json.JsonProperty("Identification", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Identification Identification { get; set; }

        [Newtonsoft.Json.JsonProperty("NameFull", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string NameFull { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class JobStatusUpdates
    {
        [Newtonsoft.Json.JsonProperty("Reference", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Reference Reference { get; set; }

        [Newtonsoft.Json.JsonProperty("RelatedWorkElementReference", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Reference> RelatedWorkElementReference { get; set; }

        [Newtonsoft.Json.JsonProperty("EventTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTime? EventTime { get; set; }

        [Newtonsoft.Json.JsonProperty("TypeCode", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public JobStatusUpdateTypeCode? TypeCode { get; set; }

        [Newtonsoft.Json.JsonProperty("OtherType", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string OtherType { get; set; }

        [Newtonsoft.Json.JsonProperty("OperativesAssigned", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<OperativesAssigned> OperativesAssigned { get; set; }

        [Newtonsoft.Json.JsonProperty("RefinedAppointmentWindow", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public RefinedAppointmentWindow RefinedAppointmentWindow { get; set; }

        [Newtonsoft.Json.JsonProperty("CustomerFeedback", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public CustomerFeedback CustomerFeedback { get; set; }

        [Newtonsoft.Json.JsonProperty("CustomerCommunicationChannelAttempted", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public CustomerCommunicationChannelAttempted CustomerCommunicationChannelAttempted { get; set; }

        [Newtonsoft.Json.JsonProperty("Comments", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Comments { get; set; }

        [Newtonsoft.Json.JsonProperty("MoreSpecificSORCode", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public WorkElement MoreSpecificSORCode { get; set; }

        [Newtonsoft.Json.JsonProperty("Evidence", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Attachment> Evidence { get; set; }

        [Newtonsoft.Json.JsonProperty("AdditionalWork", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public AdditionalWork AdditionalWork { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class OperativesAssigned
    {
        [Newtonsoft.Json.JsonProperty("Identification", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Identification Identification { get; set; }

        [Newtonsoft.Json.JsonProperty("NameFull", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string NameFull { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class RefinedAppointmentWindow
    {
        [Newtonsoft.Json.JsonProperty("Date", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(DateFormatConverter))]
        public System.DateTime? Date { get; set; }

        [Newtonsoft.Json.JsonProperty("TimeOfDay", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public TimeOfDay TimeOfDay { get; set; } = new TimeOfDay();

        [Newtonsoft.Json.JsonProperty("Reference", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Reference Reference { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class CustomerFeedback
    {
        [Newtonsoft.Json.JsonProperty("PartyProvidingFeedback", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Party PartyProvidingFeedback { get; set; }

        [Newtonsoft.Json.JsonProperty("PartyCarryingOutSurvey", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Party PartyCarryingOutSurvey { get; set; }

        [Newtonsoft.Json.JsonProperty("FeedbackSet", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<FeedbackSet> FeedbackSet { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class CustomerCommunicationChannelAttempted
    {
        [Newtonsoft.Json.JsonProperty("Channel", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Channel Channel { get; set; }

        [Newtonsoft.Json.JsonProperty("Value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Value { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class AdditionalWork
    {
        [Newtonsoft.Json.JsonProperty("AdditionalWorkOrder", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public AdditionalWorkOrder AdditionalWorkOrder { get; set; } = new AdditionalWorkOrder();

        [Newtonsoft.Json.JsonProperty("ReasonRequired", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ReasonRequired { get; set; }

        [Newtonsoft.Json.JsonProperty("Evidence", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Attachment> Evidence { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class TimeOfDay
    {
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("EarliestArrivalTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTime EarliestArrivalTime { get; set; }

        [Newtonsoft.Json.JsonProperty("LatestArrivalTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTime LatestArrivalTime { get; set; }

        [Newtonsoft.Json.JsonProperty("LatestCompletionTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTime LatestCompletionTime { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class FeedbackSet
    {
        [Newtonsoft.Json.JsonProperty("Score", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(1)]
        public System.Collections.Generic.ICollection<Score> Score { get; set; } = new System.Collections.ObjectModel.Collection<Score>();

        [Newtonsoft.Json.JsonProperty("DateTime", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTime DateTime { get; set; }

        [Newtonsoft.Json.JsonProperty("PreviousDateTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTime? PreviousDateTime { get; set; }

        [Newtonsoft.Json.JsonProperty("Description", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Description { get; set; }

        [Newtonsoft.Json.JsonProperty("Categorization", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Categorization> Categorization { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Channel
    {
        [Newtonsoft.Json.JsonProperty("Medium", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public CommunicationMediumCode? Medium { get; set; }

        [Newtonsoft.Json.JsonProperty("Code", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public CommunicationChannelCodeC0? Code { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class AdditionalWorkOrder
    {
        [Newtonsoft.Json.JsonProperty("DescriptionOfWork", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string DescriptionOfWork { get; set; }

        [Newtonsoft.Json.JsonProperty("EstimatedLaborHours", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double? EstimatedLaborHours { get; set; }

        [Newtonsoft.Json.JsonProperty("Reference", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(1)]
        public System.Collections.Generic.ICollection<Reference> Reference { get; set; } = new System.Collections.ObjectModel.Collection<Reference>();

        [Newtonsoft.Json.JsonProperty("ServiceRequestReference", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Reference> ServiceRequestReference { get; set; }

        [Newtonsoft.Json.JsonProperty("Site", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public Site Site { get; set; } = new Site();

        [Newtonsoft.Json.JsonProperty("WorkElement", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<WorkElement> WorkElement { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Score
    {
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("CurrentScore", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CurrentScore { get; set; }

        [Newtonsoft.Json.JsonProperty("Minimum", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Minimum { get; set; }

        [Newtonsoft.Json.JsonProperty("Maximum", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Maximum { get; set; }

        [Newtonsoft.Json.JsonProperty("FollowUpQuestion", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string FollowUpQuestion { get; set; }

        [Newtonsoft.Json.JsonProperty("Comment", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Comment { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Categorization
    {
        [Newtonsoft.Json.JsonProperty("Type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Type { get; set; }

        [Newtonsoft.Json.JsonProperty("Category", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Category { get; set; }

        [Newtonsoft.Json.JsonProperty("VersionUsed", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string VersionUsed { get; set; }

        [Newtonsoft.Json.JsonProperty("SubCategory", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string SubCategory { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Site
    {
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("GeographicalLocation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public GeographicalLocation GeographicalLocation { get; set; }

        [Newtonsoft.Json.JsonProperty("Property", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Property> Property { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Property
    {
        [Newtonsoft.Json.JsonProperty("Address", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public PropertyAddress Address { get; set; }

        [Newtonsoft.Json.JsonProperty("Unit", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Unit> Unit { get; set; }

        [Newtonsoft.Json.JsonProperty("MasterKeySystem", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string MasterKeySystem { get; set; }

        [Newtonsoft.Json.JsonProperty("GeographicalLocation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public GeographicalLocation GeographicalLocation { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Unit
    {
        [Newtonsoft.Json.JsonProperty("Address", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public PropertyAddress Address { get; set; }

        [Newtonsoft.Json.JsonProperty("Reference", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Reference> Reference { get; set; }

        [Newtonsoft.Json.JsonProperty("Keysafe", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Keysafe Keysafe { get; set; }


    }
}
