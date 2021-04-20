using System;
using System.Collections.Generic;
using RepairsApi.V2.Generated;

namespace RepairsApi.Tests.V2.E2ETests
{
    public static class RepairMockBuilder
    {
        public static RaiseRepair CreateFullRaiseRepair()
        {

            var request = new RaiseRepair
            {
                Priority = new Priority
                {
                    PriorityCode = 0,
                    RequiredCompletionDateTime = DateTime.UtcNow,
                    Comments = "priority comments",
                    NumberOfDays = 1.5,
                    PriorityDescription = "priority description"
                },
                AccessInformation = new AccessInformation
                {
                    Description = "description",
                    Keysafe = CreateKeysafe()
                },
                DateReported = DateTime.UtcNow,
                ParkingArrangements = "parkingArrangments",
                WorkClass = new WorkClass
                {
                    WorkClassCode = WorkClassCode._0,
                    WorkClassDescription = "workClassDescription",
                    WorkClassSubType = new WorkClassSubType
                    {
                        WorkClassSubTypeDescription = "WorkClassSubTypeDescription",
                        WorkClassSubType1 = new List<string>
                        {
                            "subType1", "subType2"
                        }
                    }
                },
                WorkType = WorkType._0,
                DescriptionOfWork = "descriptionOfWork",
                EstimatedLaborHours = 4.7,
                LocationOfRepair = "locationOfRepair",
                LocationAlert = CreateList(CreateLocationAlert, 1),
                PersonAlert = CreateList(CreatePersonAlert, 1),
                WorkElement = CreateList(CreateWorkElement, 1),
                SitePropertyUnit = CreateList(CreateSitePropertyUnit, 1)
            };

            return request;
        }

        private static Keysafe CreateKeysafe()
        {

            return new Keysafe
            {
                Code = "code",
                Location = "location"
            };
        }

        private static SitePropertyUnit CreateSitePropertyUnit()
        {
            return new SitePropertyUnit
            {
                Address = new Address
                {
                    Country = CountryCode.AD,
                    Department = "department",
                    Floor = "floor",
                    Plot = "plot",
                    Postbox = "postbox",
                    Room = "room",
                    Type = "type",
                    AddressLine = new List<string>
                    {
                        "line1", "line2", "line3"
                    },
                    BuildingName = "buildingName",
                    BuildingNumber = "builderNumber",
                    CityName = "cityName",
                    ComplexName = "complexName",
                    PostalCode = "postalCode",
                    StreetName = "streetName"
                },
                Keysafe = CreateKeysafe(),
                Reference = CreateList(CreateReference, 1)
            };
        }

        private static WorkElement CreateWorkElement()
        {

            return new WorkElement
            {
                Reference = CreateReference(),
                ContainsCapitalWork = true,
                Trade = CreateList(CreateTrade, 1),
                DependsOn = CreateList(CreateDependsOn, 1),
                RateScheduleItem = CreateList(CreateRateScheduleItem, 1),
                ServiceChargeSubject = CostSubjectCode._10
            };
        }

        private static RateScheduleItem CreateRateScheduleItem()
        {

            return new RateScheduleItem
            {
                Quantity = new Quantity
                {
                    Amount = new List<double>
                    {
                        7.2333
                    },
                    UnitOfMeasurementCode = UNECEUnitOfMeasurementCodeC0._01
                },
                CustomCode = "rateScheduledItemCustomCode",
                CustomName = "rateScheduledItemCustomName",
                M3NHFSORCode = "AA"
            };
        }

        private static DependsOn CreateDependsOn()
        {

            return new DependsOn
            {
                Timing = new Timing
                {
                    Days = 4,
                    Hours = 6,
                    Months = 2,
                    Weeks = 7
                },
                Type = DependencyTypeCode._10,
                DependsOnWorkElementReference = CreateReference()
            };
        }

        private static Trade CreateTrade()
        {

            return new Trade
            {
                Code = TradeCode.B2,
                CustomCode = "tradeCustomCode",
                CustomName = "tradeCustomName"
            };
        }

        private static PersonAlert CreatePersonAlert()
        {

            return new PersonAlert
            {
                Comments = "personAlertComment",
                Type = PersonAlertTypeCode._0
            };
        }

        private static LocationAlert CreateLocationAlert()
        {

            return new LocationAlert
            {
                Comments = "locationAlertComments",
                Type = LocationAlertTypeCode._0,
                Attachment = CreateList(CreateAttachment, 1)
            };
        }

        private static Attachment CreateAttachment()
        {
            return new Attachment
            {
                Description = "attachmentDescription",
                Filename = "attachmentFilename",
                Title = "attachmentTitle",
                CopyrightNotices = "attachmentCopyright",
                AsOfDate = DateTime.UtcNow,
                CreationDateTime = DateTime.UtcNow,
                URI = "attachmentUri",
                EmbeddedFileBinaryObject = "attachmentEmbeddedBinary",
                Reference = CreateList(CreateReference, 1)
            };
        }

        private static Reference CreateReference()
        {
            return new Reference
            {
                Description = "refDescription",
                AllocatedBy = "refAllocatedBy",
                ID = "refId"
            };
        }

        private static List<T> CreateList<T>(Func<T> createObject, int count)
        {
            var list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                list.Add(createObject());
            }

            return list;
        }
    }
}
