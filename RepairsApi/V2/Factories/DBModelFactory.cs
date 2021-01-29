using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Boundary.Response;
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
                LocationAlert = raiseRepair.LocationAlert.MapList(la => la.ToDb()),
                PersonAlert = raiseRepair.PersonAlert.MapList(pa => pa.ToDb()),
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
                LocationAlert = raiseRepair.LocationAlert.MapList(la => la.ToDb()),
                PersonAlert = raiseRepair.PersonAlert.MapList(pa => pa.ToDb()),
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
                Contact = org.Contact.MapList(c => c.ToDb()),
                Name = org.Name,
                DoingBusinessAsName = org.DoingBusinessAsName is null ? null : string.Join(';', org.DoingBusinessAsName)
            };
        }

        public static Contact ToDb(this Generated.Contact request)
        {
            return new Contact
            {
                Address = request.Address.MapList(addr => addr.ToDb()),
                Person = request.ToDbPerson(),
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
                GeographicalLocation = site.GeographicalLocation?.ToDb(),
                Name = site.Name,
                PropertyClass = site.Property.MapList(prop => prop.ToDb()),
            };
        }


        public static GeographicalLocation ToDb(this Generated.GeographicalLocation request)
        {
            try
            {
                return new GeographicalLocation
                {
                    Elevation = ParseNullableDouble(request.Elevation),
                    ElevationReferenceSystem = request.ElevationReferenceSystem.FirstOrDefault(),
                    Latitude = ParseNullableDouble(request.Latitude),
                    Longitude = ParseNullableDouble(request.Longitude),
                    PositionalAccuracy = request.PositionalAccuracy,
                    Polyline = JsonConvert.SerializeObject(request.Polyline)
                };
            }
            catch (FormatException e)
            {
                throw new NotSupportedException(Resources.InvalidGeographicalLocation, e);
            }
        }

        private static double? ParseNullableDouble(ICollection<string> strings)
        {
            string s = strings?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(s)) return null;

            return double.Parse(s);
        }

        public static PropertyClass ToDb(this Generated.Property request)
        {
            return new PropertyClass
            {
                GeographicalLocation = request.GeographicalLocation?.ToDb(),
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
            return new RateScheduleItem
            {
                CustomCode = raiseRepair.CustomCode,
                CustomName = raiseRepair.CustomName,
                Quantity = raiseRepair.Quantity?.ToDb()
            };
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

        public static AlertRegardingPerson ToDb(this Generated.PersonAlert raiseRepair)
        {
            return new AlertRegardingPerson
            {
                Comments = raiseRepair.Comments,
                Type = raiseRepair.Type
            };
        }

        public static AlertRegardingLocation ToDb(this Generated.LocationAlert raiseRepair)
        {
            return new AlertRegardingLocation
            {
                Comments = raiseRepair.Comments,
                Type = raiseRepair.Type,
                // NOTE: Attachment Not handled
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

        public static List<TResult> MapList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> map)
        {
            if (source is null) return new List<TResult>();

            return source.Select(map).ToList();
        }

        public static CustomerSatisfaction ToDb(this Generated.CustomerFeedback customerFeedback)
        {
            return new CustomerSatisfaction
            {
                FeedbackSet = customerFeedback.FeedbackSet.Select(f => f.ToDb()).ToList(),
                PartyProvidingFeedback = customerFeedback.PartyProvidingFeedback?.ToDb(),
                PartyCarryingOutSurvey = customerFeedback.PartyCarryingOutSurvey?.ToDb()
            };
        }

        public static ScoreSet ToDb(this Generated.FeedbackSet feedbackSet)
        {
            return new ScoreSet
            {
                Categorization = feedbackSet.Categorization.MapList(c => c.ToDb()),
                Description = feedbackSet.Description,
                Score = feedbackSet.Score.MapList(s => s.ToDb()),
                DateTime = feedbackSet.DateTime,
                PreviousDateTime = feedbackSet.PreviousDateTime
            };
        }

        public static Categorization ToDb(this Generated.Categorization categorization)
        {
            return new Categorization
            {
                Category = categorization.Category,
                Type = categorization.Type,
                SubCategory = categorization.SubCategory,
                VersionUsed = categorization.VersionUsed
            };
        }

        public static Score ToDb(this Generated.Score score)
        {
            return new Score
            {
                Comment = score.Comment,
                Maximum = score.Maximum,
                Minimum = score.Minimum,
                Name = score.Name,
                CurrentScore = score.CurrentScore,
                FollowUpQuestion = score.FollowUpQuestion
            };
        }

        public static Party ToDb(this Generated.Party party)
        {
            return new Party
            {
                Name = party.Name,
                Role = party.Role,
                Organization = party.Organization?.ToDb(),
                Person = party.Person?.ToDb()
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
            IEnumerable<WorkElement> workElements,
            WorkOrder workOrder)
        {
            return new JobStatusUpdate
            {
                RelatedWorkElement = workElements.ToList(),
                EventTime = DateTime.Now,
                TypeCode = jobStatusUpdate.TypeCode,
                AdditionalWork = jobStatusUpdate.AdditionalWork?.ToDb(),
                Comments = jobStatusUpdate.Comments,
                CustomerCommunicationChannelAttempted = jobStatusUpdate.CustomerCommunicationChannelAttempted?.ToDb(),
                CustomerFeedback = jobStatusUpdate.CustomerFeedback?.ToDb(),
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
                CustomerFeedback = update.CustomerFeedback?.ToDb(),
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
                OperativesUsed = request.OperativesUsed.MapList(ou => ou.ToDb(null)),
                CompletedWorkElements = request.CompletedWorkElements.MapList(cwe => cwe.ToDb()),
                JobStatusUpdates = request.JobStatusUpdates.MapList(jsu => jsu.ToDb(workOrder)),
                BillOfMaterialItem = request.BillOfMaterialItem.MapList(bom => bom.ToDb()),
                FollowOnWorkOrder = followOnWorkOrders
            };
        }

        public static Operative ToDb(this Generated.OperativesUsed operative, List<WorkElement> workElements)
        {
            return new Operative
            {
                Person = new Person
                {
                    Name = new PersonName
                    {
                        Full = operative.NameFull
                    }
                },
                Trade = operative.Trade.Select(t => t.ToDb()).ToList(),
                WorkElement = workElements
            };
        }
    }
}
