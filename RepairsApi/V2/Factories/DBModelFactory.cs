using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.Factories
{
    public static class DBModelFactory
    {
        public static WorkOrder ToDb(this Generated.RaiseRepair raiseRepair)
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
                Site = raiseRepair.SitePropertyUnit?.ToDb(),
                AccessInformation = raiseRepair.AccessInformation?.ToDb(),
                LocationAlert = raiseRepair.LocationAlert.MapList(la => la.ToDb()),
                PersonAlert = raiseRepair.PersonAlert.MapList(pa => pa.ToDb()),
                WorkElements = raiseRepair.WorkElement.MapList(we => we.ToDb())
            };
        }

        public static Site ToDb(this ICollection<Generated.SitePropertyUnit> raiseRepair)
        {
            if (raiseRepair.Count != 1) throw new NotSupportedException("Multiple addresses is not supported");

            var raiseRepairProp = raiseRepair.Single();

            return new Site
            {
                PropertyClass = new List<PropertyClass>
                {
                    new PropertyClass
                    {
                        Address = raiseRepairProp.Address?.ToDb(),
                    }
                }
            };
        }

        public static PropertyAddress ToDb(this Generated.Address raiseRepair)
        {
            return new PropertyAddress
            {
                Address = new PostalAddress
                {
                    Address = new Address
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
                    }
                }
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
                PartyProvidingFeedback = customerFeedback.PartyProvidingFeedback.ToDb(),
                PartyCarryingOutSurvey = customerFeedback.PartyCarryingOutSurvey.ToDb()
            };
        }

        public static ScoreSet ToDb(this Generated.FeedbackSet feedbackSet)
        {
            return new ScoreSet
            {
                Categorization = feedbackSet.Categorization.Select(c => c.ToDb()).ToList(),
                Description = feedbackSet.Description,
                Score = feedbackSet.Score.Select(s => s.ToDb()).ToList(),
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
                Role = party.Role
            };
        }

        public static Person ToDb(this Generated.OperativesAssigned operativesAssigned)
        {
            return new Person
            {
                Identification = operativesAssigned.Identification.ToDb(),
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
                TimeOfDay = refinedAppointmentWindow.TimeOfDay.ToDb()
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

        public static ScheduleOfRates ToDb(this ScheduleOfRatesModel sorCode)
        {
            return new ScheduleOfRates
            {
                CustomCode = sorCode.CustomCode,
                CustomName = sorCode.CustomName,
                SORContractorRef = sorCode.SORContractor.Reference,
                PriorityId = sorCode.Priority.PriorityCode
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
                AdditionalWorkOrder = additionalWork.AdditionalWorkOrder.ToDb(),
                ReasonRequired = additionalWork.ReasonRequired
            };
        }

        public static CommunicationChannel ToDb(this Generated.Channel channel)
        {
            return new CommunicationChannel { Code = channel.Code, Medium = channel.Medium };
        }

        public static Communication ToDb(this Generated.CustomerCommunicationChannelAttempted ccca)
        {
            return new Communication { Channel = ccca.Channel.ToDb(), Value = ccca.Value };
        }

        public static JobStatusUpdate ToDb(this Generated.JobStatusUpdate jobStatusUpdate,
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
    }
}
