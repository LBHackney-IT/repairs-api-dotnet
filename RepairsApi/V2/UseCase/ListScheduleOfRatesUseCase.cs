using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
#nullable enable
    public class ListScheduleOfRatesUseCase : IListScheduleOfRatesUseCase
    {
        public Task<IList<ScheduleOfRatesModel>> Execute()
        {
            IList<ScheduleOfRatesModel> sorCodes = new List<ScheduleOfRatesModel>
            {
                new ScheduleOfRatesModel
                {
                    CustomCode = "DES5R003",
                    CustomName = "Immediate call outs",
                    Priority = new Domain.SORPriority
                    {
                        PriorityCode = (int)V2.Enums.WorkPriorityCode.Immediate,
                        Description = "I - Immediate (2 hours)"
                    },
                    SORContractor = new Domain.Contractor
                    {
                        Reference = "H01"
                    }
                },
                new ScheduleOfRatesModel
                {
                    CustomCode = "DES5R004",
                    CustomName = "Emergency call out",
                    Priority = new Domain.SORPriority
                    {
                        PriorityCode = (int)V2.Enums.WorkPriorityCode.Emergency,
                        Description = "E - Emergency (24 hours)"
                    },
                    SORContractor = new Domain.Contractor
                    {
                        Reference = "H01"
                    }
                },
                new ScheduleOfRatesModel
                {
                    CustomCode = "DES5R006",
                    CustomName = "Urgent call outs",
                     Priority = new Domain.SORPriority
                    {
                        PriorityCode = (int)V2.Enums.WorkPriorityCode.Urgent,
                        Description = "U - Urgent (5 Working days)"
                    },
                    SORContractor = new Domain.Contractor
                    {
                        Reference = "H01"
                    }
                },
                new ScheduleOfRatesModel
                {
                    CustomCode = "DES5R005",
                    CustomName = "Normal call outs",
                    Priority = new Domain.SORPriority
                    {
                        PriorityCode = (int)V2.Enums.WorkPriorityCode.Normal,
                        Description = "N - Normal (21 working days)"
                    },
                    SORContractor = new Domain.Contractor
                    {
                        Reference = "H01"
                    }
                },
                new ScheduleOfRatesModel
                {
                    CustomCode = "DES5R013",
                    CustomName = "Inspect additional sec entrance",
                    Priority = new Domain.SORPriority
                    {
                        PriorityCode = (int)V2.Enums.WorkPriorityCode.Inspection,
                        Description = "Inspection"
                    },
                    SORContractor = new Domain.Contractor
                    {
                        Reference = "H01"
                    }
                }
            };
            return Task.FromResult(sorCodes);
        }
    }
}
