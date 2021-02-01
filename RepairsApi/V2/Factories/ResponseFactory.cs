using RepairsApi.V2.Boundary;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Domain;
using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure.Extensions;
using Address = RepairsApi.V2.Domain.Address;
using SORPriority = RepairsApi.V2.Domain.SORPriority;
using WorkElement = RepairsApi.V2.Generated.WorkElement;

namespace RepairsApi.V2.Factories
{
    public static class ResponseFactory
    {
        public static CautionaryAlertResponseList ToResponse(this AlertList domain)
        {
            return new CautionaryAlertResponseList()
            {
                PropertyReference = domain.PropertyAlerts.PropertyReference,
                LocationAlert = domain.PropertyAlerts.Alerts.Select(alert => alert.ToResponse()).ToList(),
                PersonAlert = domain.PersonAlerts.Alerts.Select(alert => alert.ToResponse()).ToList()
            };
        }

        public static CautionaryAlertViewModel ToResponse(this Alert domain)
        {
            return new CautionaryAlertViewModel
            {
                Type = domain.AlertCode,
                Comments = domain.Description,
                EndDate = domain.EndDate,
                StartDate = domain.StartDate
            };
        }

        public static PropertyViewModel ToResponse(this PropertyModel domain, TenureInformation tenure)
        {
            return new PropertyViewModel
            {
                CanRaiseRepair = (tenure is null) || tenure.CanRaiseRepair, // If there is no tenure then we CAN raise repairs
                PropertyReference = domain.PropertyReference,
                Address = domain.Address.ToResponse(),
                HierarchyType = domain.HierarchyType.ToResponse()
            };
        }

        public static PropertyListItem ToResponseListItem(this PropertyModel domain)
        {
            return new PropertyListItem
            {
                PropertyReference = domain.PropertyReference,
                Address = domain.Address.ToResponse(),
                HierarchyType = domain.HierarchyType.ToResponse()
            };
        }

        public static AddressViewModel ToResponse(this Address domain)
        {
            return new AddressViewModel
            {
                AddressLine = domain.AddressLine,
                PostalCode = domain.PostalCode,
                ShortAddress = domain.ShortAddress,
                StreetSuffix = domain.StreetSuffix
            };
        }

        public static HierarchyTypeViewModel ToResponse(this HierarchyType domain)
        {
            return new HierarchyTypeViewModel
            {
                LevelCode = domain.LevelCode,
                SubTypeCode = domain.SubTypeCode,
                SubTypeDescription = domain.SubTypeDescription
            };
        }

        public static PropertyResponse ToResponse(this PropertyWithAlerts domain)
        {
            return new PropertyResponse
            {
                Property = domain.PropertyModel.ToResponse(domain.Tenure),
                Alerts = new AlertsViewModel
                {
                    LocationAlert = domain.LocationAlerts.Select(alert => alert.ToResponse()).ToList(),
                    PersonAlert = domain.PersonAlerts.Select(alert => alert.ToResponse()).ToList(),
                },
                Tenure = domain.Tenure.ToResponse()
            };
        }


        public static TenureViewModel ToResponse(this TenureInformation domain)
        {
            if (domain == null) return null;

            return new TenureViewModel
            {
                TypeCode = domain.TypeCode,
                TypeDescription = domain.TypeDescription,
                CanRaiseRepair = domain.CanRaiseRepair
            };
        }

        public static List<PropertyListItem> ToResponse(this IEnumerable<PropertyModel> domainList)
        {
            return domainList.Select(domain => domain.ToResponseListItem()).ToList();
        }

        public static WorkOrderResponse ToResponse(this Infrastructure.WorkOrder workOrder)
        {
            Infrastructure.PropertyClass propertyClass = workOrder.Site?.PropertyClass.FirstOrDefault();
            string addressLine = propertyClass?.Address?.AddressLine;
            return new WorkOrderResponse
            {
                Reference = workOrder.Id,
                Description = workOrder.DescriptionOfWork,
                Priority = workOrder.WorkPriority.PriorityDescription,
                Property = addressLine,
                DateRaised = workOrder.DateRaised,
                PropertyReference = workOrder.Site?.PropertyClass.FirstOrDefault()?.PropertyReference,
                Target = workOrder.WorkPriority.RequiredCompletionDateTime,
                PriorityCode = workOrder.WorkPriority.PriorityCode,
                LastUpdated = null,
                Owner = workOrder.AssignedToPrimary?.Name,
                RaisedBy = "Dummy Agent",
                CallerName = workOrder.Customer?.Person?.Name?.Full,
                CallerNumber = workOrder.Customer?.Person?.Communication?.Where(cc => cc.Channel.Medium == Generated.CommunicationMediumCode._20 /* Audio */).FirstOrDefault()
                    ?.Value,
                Status = workOrder.GetStatus()
            };
        }

        public static WorkOrderListItem ToListItem(this Infrastructure.WorkOrder workOrder)
        {
            Infrastructure.PropertyClass propertyClass = workOrder.Site?.PropertyClass.FirstOrDefault();
            string addressLine = propertyClass?.Address?.AddressLine;
            return new WorkOrderListItem
            {
                Reference = workOrder.Id,
                Description = workOrder.DescriptionOfWork,
                Owner = workOrder.AssignedToPrimary?.Name,
                Priority = workOrder.WorkPriority.PriorityDescription,
                Property = addressLine,
                DateRaised = workOrder.DateRaised,
                LastUpdated = null,
                PropertyReference = workOrder.Site?.PropertyClass.FirstOrDefault()?.PropertyReference,
                TradeCode = workOrder.WorkElements.FirstOrDefault()?.Trade.FirstOrDefault()?.CustomCode,
                Status = workOrder.GetStatus()
            };
        }

        public static ScheduleOfRatesModel ToResponse(this Infrastructure.ScheduleOfRates sorCode)
        {
            return new ScheduleOfRatesModel
            {
                CustomCode = sorCode.CustomCode,
                CustomName = sorCode.CustomName,
                SORContractor = new Contractor
                {
                    Reference = sorCode.SORContractorRef
                },
                Priority = new SORPriority
                {
                    Description = sorCode.Priority.Description,
                    PriorityCode = sorCode.Priority.PriorityCode
                }
            };
        }

        public static WorkElement ToResponse(this Infrastructure.WorkElement workElement)
        {
            return new WorkElement
            {
                Trade = workElement.Trade.Select(t => t.ToResponse()).ToList(),
                DependsOn = workElement.DependsOn.Select(d => d.ToResponse()).ToList(),
                ContainsCapitalWork = workElement.ContainsCapitalWork,
                RateScheduleItem = workElement.RateScheduleItem.Select(rsi => rsi.ToResponse()).ToList(),
                ServiceChargeSubject = workElement.ServiceChargeSubject
            };
        }

        public static Trade ToResponse(this Infrastructure.Trade trade)
        {
            return new Trade
            {
                Code = trade.Code.Value,
                CustomCode = trade.CustomCode,
                CustomName = trade.CustomName
            };
        }

        public static DependsOn ToResponse(this Infrastructure.WorkElementDependency dependency)
        {
            return new DependsOn
            {
                Timing = new Timing
                {
                    Days = dependency.Dependency.Duration.Value.Offset.Days
                },
                Type = dependency.Dependency.Type,
                DependsOnWorkElementReference = new Reference
                {
                    ID = dependency.DependsOnWorkElement.Id.ToString()
                }
            };
        }

        public static RateScheduleItem ToResponse(this Infrastructure.RateScheduleItem rateScheduleItem)
        {
            return new RateScheduleItem
            {
                Quantity = rateScheduleItem.Quantity.ToResponse(),
                CustomCode = rateScheduleItem.CustomCode,
                CustomName = rateScheduleItem.CustomName,
                M3NHFSORCode = rateScheduleItem.M3NHFSORCode
            };
        }

        public static Quantity ToResponse(this Infrastructure.Quantity quantity)
        {
            return new Quantity
            {
                Amount = new List<double>
                {
                    quantity.Amount
                },
                UnitOfMeasurementCode = quantity.UnitOfMeasurementCode
            };
        }

        public static IEnumerable<WorkOrderItemViewModel> ToResponse(this IEnumerable<WorkOrderTask> domain)
        {
            return domain.Select(wot => wot.ToResponse());
        }

        public static WorkOrderItemViewModel ToResponse(this WorkOrderTask domain)
        {
            return new WorkOrderItemViewModel
            {
                Quantity = domain.Quantity,
                Code = domain.Id,
                Cost = domain.Cost,
                DateAdded = domain.DateAdded,
                Description = domain.Description,
                Status = domain.Status
            };
        }
    }
}
