
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class ScoreSet
    {
        [Key] public int Id { get; set; }
        public virtual List<Score> Score { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime PreviousDateTime { get; set; }
        public string Description { get; set; }
        public virtual List<Categorization> Categorization { get; set; }
    }


}

