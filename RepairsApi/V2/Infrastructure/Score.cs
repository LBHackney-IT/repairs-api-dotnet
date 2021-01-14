using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Score
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string CurrentScore { get; set; }
        public string Minimum { get; set; }
        public string Maximum { get; set; }
        public string FollowUpQuestion { get; set; }
        public string Comment { get; set; }
    }
}

