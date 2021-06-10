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
        public string AgentEmail { get; set; }
        public WorkStatusCode StatusCode { get; set; } = WorkStatusCode.Open;
        public ReasonCode Reason { get; set; } = ReasonCode.FullyFunded;
        public virtual List<WorkOrderOperative> WorkOrderOperatives { get; set; }
        public virtual List<Operative> AssignedOperatives { get; set; }
    }

    public class WorkOrderOperative
    {
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public int OperativeId { get; set; }
        public virtual Operative Operative { get; set; }

        public override bool Equals(object obj)
        {
            return obj is WorkOrderOperative other && WorkOrderId == other.WorkOrderId && OperativeId == other.OperativeId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(WorkOrderId, OperativeId);
        }
    }

    public enum WorkStatusCode
    {
        /// <summary>
        /// Acct Hold: Work Is On Accounting Hold - See Reason Code. Active.
        /// </summary>
        AcctHold = 10,

        /// <summary>
        /// Assigned: Work Is Assigned To A Group Or Individual. Active.
        /// </summary>
        Assigned = 20,

        /// <summary>
        /// Canceled: Work Is Canceled After Work Began. Inactive.
        /// </summary>
        Canceled = 30,

        /// <summary>
        /// Closed: Work Is Complete And Record Is Complete - Wo Is Closed. Inactive.
        /// </summary>
        Closed = 40,

        /// <summary>
        /// Complete: Work Is Complete But Record Needs To Be Updated. Inactive.
        /// </summary>
        Complete = 50,

        /// <summary>
        /// Estimating: Work Is Being Estimated. Active.
        /// </summary>
        Estimating = 60,

        /// <summary>
        /// Hold: Work Is On Hold - See Reason Code. Active
        /// </summary>
        Hold = 70,

        /// <summary>
        /// Open: Work Order Is Open - Initial Status For All Work Orders. Active.
        /// </summary>
        Open = 80,

        /// <summary>
        /// Pend App: Work Order Has a paending Variation . Active.
        /// </summary>
        VariationPendingApproval = 90,

        /// <summary>
        /// Pend Design: Work Order Is Pending Design Or Design Documents. Active.
        /// </summary>
        PendDesign = 100,

        /// <summary>
        /// Pend Material: Work Order Is Pending Materials. Active.
        /// </summary>
        PendMaterial = 110,

        /// <summary>
        /// Scheduled: Work Is Scheduled Using A Scheduling Tool. Active.
        /// </summary>
        Scheduled = 120,

        /// <summary>
        /// Superceded: Work Order Is Superceded By Another. Inactive.
        /// </summary>
        Superceded = 130,

        // Extensions
        NoAccess = 1000,
        PendingApproval = 1010,
        VariationApproved = 1080,
        VariationRejected = 1090,
    }

    public enum ReasonCode
    {
        /// <summary>
        /// No Budget To Perform The Requested Work
        /// </summary>
        NoBudget = 10,

        /// <summary>
        /// Low Priority: Work Considered A Low Priority
        /// </summary>
        LowPriority = 20,

        /// <summary>
        /// Fully Funded: Work Is Fully Funded And Ready For Next Step
        /// </summary>
        FullyFunded = 30,

        /// <summary>
        /// Partially Funded: Work Is Partially Funded And On Hold Until Fully Funded
        /// </summary>
        PartiallyFunded = 40,

        /// <summary>
        /// Schedule Conflict Exists With This Work Request
        /// </summary>
        ScheduleConflict = 50,

        /// <summary>
        /// No Approval: Work Was Not Approved
        /// </summary>
        NoApproval = 60,

        /// <summary>
        /// Approved: All Required Approvals Are Present
        /// </summary>
        Approved = 70,

        /// <summary>
        /// Change Priority Changed Either Escalated Or De-Escalated
        /// </summary>
        Priority = 80,

        /// <summary>
        /// Pending authorisation
        /// </summary>
        VariationPendingAuthorisation = 90
    }

    [Owned]
    public class WorkOrderAccessInformation
    {
        public string Description { get; set; }
        public virtual KeySafe Keysafe { get; set; }
    }
}
