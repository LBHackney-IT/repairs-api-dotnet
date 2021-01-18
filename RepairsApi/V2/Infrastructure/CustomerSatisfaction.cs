using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class CustomerSatisfaction
    {
        [Key] public int Id { get; set; }
        public virtual Party PartyProvidingFeedback { get; set; }
        public virtual Party PartyCarryingOutSurvey { get; set; }
        public virtual List<ScoreSet> FeedbackSet { get; set; }
    }
}

