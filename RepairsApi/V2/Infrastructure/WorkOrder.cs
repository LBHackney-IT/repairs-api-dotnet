using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.Infrastructure
{
    public class WorkOrder
    {
        [Key] public int Id { get; set; }
        public string DescriptionOfWork { get; set; }
        public double? EstimatedLaborHours { get; set; }
        public WorkType? WorkType { get; set; }
        public string ParkingArrangements { get; set; }
        public string LocationOfRepair { get; set; }
        public DateTime? DateRaised { get; set; }
        public DateTime? DateReported { get; set; }
        public virtual WorkClass WorkClass { get; set; }
        public virtual Organization InstructedBy { get; set; }
        public virtual Party AssignedToPrimary { get; set; }

        public virtual Party Customer { get; set; }
        public virtual WorkPriority WorkPriority { get; set; }
        public virtual Site Site { get; set; }
        public virtual WorkOrderAccessInformation AccessInformation { get; set; }
        public virtual List<WorkElement> WorkElements { get; set; }
        public virtual List<AlertRegardingPerson> PersonAlert { get; set; }
        public virtual List<AlertRegardingLocation> LocationAlert { get; set; }
        public virtual WorkOrderComplete WorkOrderComplete { get; set; }
        public virtual List<JobStatusUpdate> JobStatusUpdates { get; set; }

        // Extensions
        public string AgentName { get; set; }
        public WorkStatusCode StatusCode { get; set; } = WorkStatusCode.Open;
        public ReasonCode Reason { get; set; } = ReasonCode.FullyFunded;
    }

    public enum WorkStatusCode
    {
        AcctHold = 10, // Acct Hold: Work Is On Accounting Hold - See Reason Code. Active.
        Assigned = 20, // Assigned: Work Is Assigned To A Group Or Individual. Active.
        Canceled = 30, // Canceled: Work Is Canceled After Work Began. Inactive.
        Closed = 40, // Closed: Work Is Complete And Record Is Complete - Wo Is Closed. Inactive.
        Complete = 50, // Complete: Work Is Complete But Record Needs To Be Updated. Inactive.
        Estimating = 60, // Estimating: Work Is Being Estimated. Active.
        Hold = 70, // Hold: Work Is On Hold - See Reason Code. Active
        Open = 80, // Open: Work Order Is Open - Initial Status For All Work Orders. Active.
        PendApp = 90, // Pend App: Work Order Is Pending Approval. Active.
        PendDesign = 100, // Pend Design: Work Order Is Pending Design Or Design Documents. Active.
        PendMaterial = 110, // Pend Material: Work Order Is Pending Materials. Active.
        Scheduled = 120, // Scheduled: Work Is Scheduled Using A Scheduling Tool. Active.
        Superceded = 130, // Superceded: Work Order Is Superceded By Another. Inactive.

        // Extensions
        NoAccess = 1000,
        VariationApproved = 1080,
        VariationRejected = 1090,
    }

    public enum ReasonCode
    {
        NoBudget = 10,	   //No Budget To Perform The Requested Work
        LowPriority = 20,      //Low Priority: Work Considered A Low Priority
        FullyFunded = 30,      //Fully Funded: Work Is Fully Funded And Ready For Next Step
        PartiallyFunded = 40,      //Partially Funded: Work Is Partially Funded And On Hold Until Fully Funded
        ScheduleConflict = 50,     //Schedule Conflict Exists With This Work Request
        NoApproval = 60,    //No Approval: Work Was Not Approved
        Approved = 70,      //Approved: All Required Approvals Are Present
        Priority = 80,   //Change Priority Changed Either Escalated Or De-Escalated
        PendingAuthorisation = 90 //Pending authorisation
    }

    [Owned]
    public class WorkOrderAccessInformation
    {
        public string Description { get; set; }
        public virtual KeySafe Keysafe { get; set; }
    }
}
