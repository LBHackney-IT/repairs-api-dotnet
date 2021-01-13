using System.ComponentModel;


namespace RepairsApi.V2.Enums
{
    public enum WorkPriorityCode
    {
        [Description("Immediate")] Immediate = 1,
        [Description("Emergency")] Emergency = 2,
        [Description("Urgent")] Urgent = 3,
        [Description("Normal")] Normal = 4,
        [Description("Inspection")] Inspection = 5
    }
}
