﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RepairsApi.V2 {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RepairsApi.V2.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This action is not supported.
        /// </summary>
        public static string ActionUnsupported {
            get {
                return ResourceManager.GetString("ActionUnsupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot complete a work order before assigning an operative.
        /// </summary>
        public static string CannotCompleteWithNoOperative {
            get {
                return ResourceManager.GetString("CannotCompleteWithNoOperative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot complete a work order twice.
        /// </summary>
        public static string CannotCompleteWorkOrderTwice {
            get {
                return ResourceManager.GetString("CannotCompleteWorkOrderTwice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot Resume Job.
        /// </summary>
        public static string CannotResumeJob {
            get {
                return ResourceManager.GetString("CannotResumeJob", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find contacts.
        /// </summary>
        public static string Contacts_Not_Found {
            get {
                return ResourceManager.GetString("Contacts Not Found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to fetch contacts.
        /// </summary>
        public static string ContactsFailure {
            get {
                return ResourceManager.GetString("ContactsFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Work Order Created.
        /// </summary>
        public static string CreatedWorkOrder {
            get {
                return ResourceManager.GetString("CreatedWorkOrder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GeographicalLocation.Elevation, GeographicalLocation.Latitude and GeographicalLocation.Longitude must contain a single string that is a valid decimal number.
        /// </summary>
        public static string InvalidGeographicalLocation {
            get {
                return ResourceManager.GetString("InvalidGeographicalLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You do not have the correct permissions for this action.
        /// </summary>
        public static string InvalidPermissions {
            get {
                return ResourceManager.GetString("InvalidPermissions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to fetch location alerts.
        /// </summary>
        public static string LocationAlertsFailure {
            get {
                return ResourceManager.GetString("LocationAlertsFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not Authorised to cancel jobs.
        /// </summary>
        public static string NotAuthorisedToCancel {
            get {
                return ResourceManager.GetString("NotAuthorisedToCancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not Authorised to close jobs.
        /// </summary>
        public static string NotAuthorisedToClose {
            get {
                return ResourceManager.GetString("NotAuthorisedToClose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to fetch person alerts.
        /// </summary>
        public static string PersonAlertsFailure {
            get {
                return ResourceManager.GetString("PersonAlertsFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to fetch properties.
        /// </summary>
        public static string PropertiesFailure {
            get {
                return ResourceManager.GetString("PropertiesFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find property.
        /// </summary>
        public static string Property_Not_Found {
            get {
                return ResourceManager.GetString("Property Not Found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to fetch property.
        /// </summary>
        public static string PropertyFailure {
            get {
                return ResourceManager.GetString("PropertyFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Variation rejected: .
        /// </summary>
        public static string RejectedVariationPrepend {
            get {
                return ResourceManager.GetString("RejectedVariationPrepend", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to fetch tenancy.
        /// </summary>
        public static string TenancyFailure {
            get {
                return ResourceManager.GetString("TenancyFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Un supported Priority character.
        /// </summary>
        public static string UnsupportedUHPriority {
            get {
                return ResourceManager.GetString("UnsupportedUHPriority", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsupported work order complete job status update code.
        /// </summary>
        public static string UnsupportedWorkOrderUpdate {
            get {
                return ResourceManager.GetString("UnsupportedWorkOrderUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Variation reason: .
        /// </summary>
        public static string VariationReason {
            get {
                return ResourceManager.GetString("VariationReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Authorisation rejected: .
        /// </summary>
        public static string WorkOrderAuthorisationRejected {
            get {
                return ResourceManager.GetString("WorkOrderAuthorisationRejected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Work order is not pending approval.
        /// </summary>
        public static string WorkOrderNotPendingApproval {
            get {
                return ResourceManager.GetString("WorkOrderNotPendingApproval", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Work order is not scheduled.
        /// </summary>
        public static string WorkOrderNotScheduled {
            get {
                return ResourceManager.GetString("WorkOrderNotScheduled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No WorkPriorityCode provided.
        /// </summary>
        public static string WorkPriorityCodeMissing {
            get {
                return ResourceManager.GetString("WorkPriorityCodeMissing", resourceCulture);
            }
        }
    }
}
