using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RepairsApi.V2.Factories
{
    public static class DBModelFactory
    {
        public static WorkOrder ToDb(this Generated.RaiseRepair raiseRepair)
        {
            if (raiseRepair.SitePropertyUnit?.Count != 1) throw new NotSupportedException("A single address must be provided");

            return new WorkOrder
            {
                DescriptionOfWork = raiseRepair.DescriptionOfWork,
                DateReported = raiseRepair.DateReported,
                EstimatedLaborHours = raiseRepair.EstimatedLaborHours,
                ParkingArrangements = raiseRepair.ParkingArrangements,
                LocationOfRepair = raiseRepair.LocationOfRepair,
                WorkType = raiseRepair.WorkType,
                WorkPriority = raiseRepair.Priority?.ToDb(),
                WorkClass = raiseRepair.WorkClass?.ToDb(),
                Site = raiseRepair.SitePropertyUnit?.Single().ToDb(),
                AccessInformation = raiseRepair.AccessInformation?.ToDb(),
                WorkElements = raiseRepair.WorkElement.MapList(we => we.ToDb())
            };
        }

        public static WorkOrder ToDb(this Generated.ScheduleRepair raiseRepair)
        {
            return new WorkOrder
            {
                DescriptionOfWork = raiseRepair.DescriptionOfWork,
                DateReported = raiseRepair.DateReported,
                EstimatedLaborHours = raiseRepair.EstimatedLaborHours,
                ParkingArrangements = raiseRepair.ParkingArrangements,
                LocationOfRepair = raiseRepair.LocationOfRepair,
                WorkType = raiseRepair.WorkType,
                WorkPriority = raiseRepair.Priority?.ToDb(),
                WorkClass = raiseRepair.WorkClass?.ToDb(),
                AccessInformation = raiseRepair.AccessInformation?.ToDb(),
                WorkElements = raiseRepair.WorkElement.MapList(we => we.ToDb()),
                Site = raiseRepair.Site?.ToDb(),
                AssignedToPrimary = raiseRepair.AssignedToPrimary?.ToDb(),
                InstructedBy = raiseRepair.InstructedBy?.ToDb(),
                Customer = raiseRepair.Customer?.ToDb(),
                DateRaised = raiseRepair.DateRaised
            };
        }

        public static Organization ToDb(this Generated.Organization org)
        {
            return new Organization
            {
                Name = org.Name,
                DoingBusinessAsName = org.DoingBusinessAsName is null ? null : string.Join(';', org.DoingBusinessAsName)
            };
        }

        public static Person ToDbPerson(this Generated.Contact request)
        {
            return new Person
            {
                AliasNames = request.Alias.MapList(pn => pn.ToDb()),
                Communication = request.Communication.MapList(c => c.ToDb()),
                Name = request.Name?.ToDb()
            };
        }

        public static Person ToDb(this Generated.Person request)
        {
            return new Person
            {
                AliasNames = request.Alias.MapList(pn => pn.ToDb()),
                Communication = request.Communication.MapList(c => c.ToDb()),
                Name = request.Name?.ToDb()
            };
        }


        public static Communication ToDb(this Generated.Communication request)
        {
            return new Communication
            {
                Channel = request.Channel?.ToDb(),
                Description = request.Description,
                NotAvailable = request.NotAvailable,
                Value = request.Value
            };
        }

        public static PersonName ToDb(this Generated.PersonName request)
        {
            return new PersonName
            {
                Family = request.Family,
                FamilyPrefix = request.FamilyPrefix,
                Full = request.Full,
                Given = request.Given,
                Initials = request.Initials,
                Middle = request.Middle,
                Title = request.Title
            };
        }

        public static PropertyAddress ToDb(this Generated.PropertyAddress request)
        {
            return new PropertyAddress
            {
                AddressLine = string.Join(';', request.AddressLine),
                BuildingName = request.BuildingName,
                BuildingNumber = request.BuildingNumber,
                CityName = request.CityName,
                ComplexName = request.ComplexName,
                Country = request.Country,
                Department = request.Department,
                Floor = request.Floor,
                Plot = request.Plot,
                PostalCode = request.PostalCode,
                Postbox = request.Postbox,
                Room = request.Room,
                StreetName = request.StreetName,
                Type = request.Type
            };
        }

        public static Site ToDb(this Generated.Site site)
        {
            return new Site
            {
                Name = site.Name,
                PropertyClass = site.Property.MapList(prop => prop.ToDb()),
            };
        }

        public static PropertyClass ToDb(this Generated.Property request)
        {
            return new PropertyClass
            {
                MasterKeySystem = request.MasterKeySystem,
                Address = request.Address?.ToDb(),
                Unit = request.Unit.MapList(u => u.ToDb()),
                PropertyReference = request.PropertyReference
            };
        }

        public static Unit ToDb(this Generated.Unit request)
        {
            return new Unit
            {
                Address = request.Address?.ToDb(),
                KeySafe = request.Keysafe?.ToDb()
            };
        }

        public static Site ToDb(this Generated.SitePropertyUnit raiseRepair)
        {
            return new Site
            {
                PropertyClass = new List<PropertyClass>
                {
                    new PropertyClass
                    {
                        Address = raiseRepair.Address?.ToDb(),
                        PropertyReference = raiseRepair.Reference?.FirstOrDefault()?.ID
                    }
                }
            };
        }

        public static PropertyAddress ToDb(this Generated.Address raiseRepair)
        {
            return new PropertyAddress
            {
                AddressLine = string.Join(';', raiseRepair.AddressLine),
                BuildingName = raiseRepair.BuildingName,
                BuildingNumber = raiseRepair.BuildingNumber,
                CityName = raiseRepair.CityName,
                ComplexName = raiseRepair.ComplexName,
                Country = raiseRepair.Country,
                Department = raiseRepair.Department,
                Floor = raiseRepair.Floor,
                Plot = raiseRepair.Plot,
                PostalCode = raiseRepair.PostalCode,
                Postbox = raiseRepair.Postbox,
                Room = raiseRepair.Room,
                StreetName = raiseRepair.StreetName,
                Type = raiseRepair.Type
            };
        }

        public static WorkElement ToDb(this Generated.WorkElement raiseRepair)
        {
            return new WorkElement
            {
                ContainsCapitalWork = raiseRepair.ContainsCapitalWork,
                ServiceChargeSubject = raiseRepair.ServiceChargeSubject,
                Trade = raiseRepair.Trade.MapList(t => t.ToDb()),
                RateScheduleItem = raiseRepair.RateScheduleItem.MapList(rsi => rsi.ToDb())
            };
        }

        public static RateScheduleItem ToDb(this Generated.RateScheduleItem raiseRepair)
        {
            RateScheduleItem rateScheduleItem = new RateScheduleItem
            {
                CustomCode = raiseRepair.CustomCode,
                CustomName = raiseRepair.CustomName,
                Quantity = raiseRepair.Quantity?.ToDb(),
                DateCreated = DateTime.UtcNow,
            };

            if (!string.IsNullOrWhiteSpace(raiseRepair.Id) && Guid.TryParse(raiseRepair.Id, out var id))
            {
                rateScheduleItem.OriginalId = id;
            }

            return rateScheduleItem;
        }

        public static Quantity ToDb(this Generated.Quantity raiseRepair)
        {
            if (raiseRepair.Amount.Count != 1) throw new NotSupportedException("Multiple amounts is not supported");

            return new Quantity
            {
                Amount = raiseRepair.Amount.Single(),
                UnitOfMeasurementCode = raiseRepair.UnitOfMeasurementCode
            };
        }

        public static Trade ToDb(this Generated.Trade raiseRepair)
        {
            return new Trade
            {
                Code = raiseRepair.Code,
                CustomCode = raiseRepair.CustomCode,
                CustomName = raiseRepair.CustomName
            };
        }

        public static WorkClass ToDb(this Generated.WorkClass raiseRepair)
        {
            return new WorkClass
            {
                WorkClassCode = raiseRepair.WorkClassCode,
                WorkClassDescription = raiseRepair.WorkClassDescription,
                WorkClassSubType = raiseRepair.WorkClassSubType?.ToDb()
            };
        }

        public static WorkClassSubType ToDb(this Generated.WorkClassSubType raiseRepair)
        {
            return new WorkClassSubType
            {
                WorkClassSubTypeDescription = raiseRepair.WorkClassSubTypeDescription,
                WorkClassSubTypeName = string.Join(',', raiseRepair.WorkClassSubType1)
            };
        }

        public static WorkOrderAccessInformation ToDb(this Generated.AccessInformation raiseRepair)
        {
            return new WorkOrderAccessInformation
            {
                Description = raiseRepair.Description,
                Keysafe = raiseRepair.Keysafe?.ToDb()
            };
        }

        public static KeySafe ToDb(this Generated.Keysafe raiseRepair)
        {
            return new KeySafe
            {
                Code = raiseRepair.Code,
                Location = raiseRepair.Location
            };
        }

        public static WorkPriority ToDb(this Generated.Priority raiseRepair)
        {
            return new WorkPriority
            {
                PriorityCode = raiseRepair.PriorityCode,
                RequiredCompletionDateTime = raiseRepair.RequiredCompletionDateTime,
                Comments = raiseRepair.Comments,
                PriorityDescription = raiseRepair.PriorityDescription,
                NumberOfDays = raiseRepair.NumberOfDays
            };
        }

        public static Party ToDb(this Generated.Party party)
        {
            return new Party
            {
                Name = party.Name,
                Role = party.Role,
                Organization = party.Organization?.ToDb(),
                Person = party.Person?.ToDb(),
                ContractorReference = party.Organization?.Reference?.FirstOrDefault()?.ID
            };
        }

        public static Person ToDb(this Generated.OperativesAssigned operativesAssigned)
        {
            return new Person
            {
                Identification = operativesAssigned.Identification?.ToDb(),
                Name = new PersonName
                {
                    Full = operativesAssigned.NameFull
                }
            };
        }

        public static Identification ToDb(this Generated.Identification identification)
        {
            return new Identification
            {
                Number = identification.Number,
                Type = identification.Type
            };
        }

        public static Appointment ToDb(this Generated.RefinedAppointmentWindow refinedAppointmentWindow)
        {
            return new Appointment
            {
                Date = refinedAppointmentWindow.Date,
                TimeOfDay = refinedAppointmentWindow.TimeOfDay?.ToDb()
            };
        }

        public static AppointmentTimeOfDay ToDb(this Generated.TimeOfDay timeOfDay)
        {
            return new AppointmentTimeOfDay
            {
                Name = timeOfDay.Name,
                EarliestArrivalTime = timeOfDay.EarliestArrivalTime,
                LatestArrivalTime = timeOfDay.LatestArrivalTime,
                LatestCompletionTime = timeOfDay.LatestCompletionTime
            };
        }

        public static WorkOrder ToDb(this Generated.AdditionalWorkOrder additionalWorkOrder)
        {
            return new WorkOrder
            {
                DescriptionOfWork = additionalWorkOrder.DescriptionOfWork,
                EstimatedLaborHours = additionalWorkOrder.EstimatedLaborHours,
                WorkElements = additionalWorkOrder.WorkElement.MapList(we => we.ToDb())
            };
        }

        public static AdditionalWork ToDb(this Generated.AdditionalWork additionalWork)
        {
            return new AdditionalWork
            {
                AdditionalWorkOrder = additionalWork.AdditionalWorkOrder?.ToDb(),
                ReasonRequired = additionalWork.ReasonRequired
            };
        }

        public static CommunicationChannel ToDb(this Generated.Channel channel)
        {
            return new CommunicationChannel
            {
                Code = channel.Code,
                Medium = channel.Medium
            };
        }

        public static Communication ToDb(this Generated.CustomerCommunicationChannelAttempted ccca)
        {
            return new Communication
            {
                Channel = ccca.Channel?.ToDb(),
                Value = ccca.Value
            };
        }

        public static JobStatusUpdate ToDb(
            this Generated.JobStatusUpdate jobStatusUpdate,
            WorkOrder workOrder)
        {
            return new JobStatusUpdate
            {
                EventTime = DateTime.UtcNow,
                TypeCode = jobStatusUpdate.TypeCode,
                AdditionalWork = jobStatusUpdate.AdditionalWork?.ToDb(),
                Comments = jobStatusUpdate.Comments,
                CustomerCommunicationChannelAttempted = jobStatusUpdate.CustomerCommunicationChannelAttempted?.ToDb(),
                MoreSpecificSORCode = jobStatusUpdate.MoreSpecificSORCode?.ToDb(),
                OperativesAssigned = jobStatusUpdate.OperativesAssigned?.Select(oa => oa.ToDb()).ToList(),
                OtherType = jobStatusUpdate.OtherType,
                RefinedAppointmentWindow = jobStatusUpdate.RefinedAppointmentWindow?.ToDb(),
                RelatedWorkOrder = workOrder
            };
        }

        public static JobStatusUpdate ToDb(this Generated.JobStatusUpdates update, WorkOrder workOrder)
        {
            return new JobStatusUpdate
            {
                Comments = update.Comments,
                EventTime = update.EventTime,
                OperativesAssigned = update.OperativesAssigned?.Select(oa => oa.ToDb()).ToList(),
                OtherType = update.OtherType,
                TypeCode = update.TypeCode,
                RefinedAppointmentWindow = update.RefinedAppointmentWindow?.ToDb(),
                RelatedWorkOrder = workOrder
            };
        }

        public static WorkOrderComplete ToDb(this Generated.WorkOrderComplete request, WorkOrder workOrder, List<WorkOrder> followOnWorkOrders)
        {
            return new WorkOrderComplete
            {
                WorkOrder = workOrder,
                CompletedWorkElements = request.CompletedWorkElements.MapList(cwe => cwe.ToDb()),
                JobStatusUpdates = request.JobStatusUpdates.MapList(jsu => jsu.ToDb(workOrder)),
                BillOfMaterialItem = request.BillOfMaterialItem.MapList(bom => bom.ToDb()),
                FollowOnWorkOrder = followOnWorkOrders,
                ClosedTime = request.ClosedTime
            };
        }
    }
}
