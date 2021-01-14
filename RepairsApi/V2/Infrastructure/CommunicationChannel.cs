using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class CommunicationChannel
    {
        [Key] public int Id { get; set; }
        public CommunicationMediumCode Medium { get; set; }
        public CommunicationChannelCode Code { get; set; }
    }


}

