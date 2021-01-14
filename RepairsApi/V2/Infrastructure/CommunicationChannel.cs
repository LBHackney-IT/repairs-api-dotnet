using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Generated;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class CommunicationChannel
    {
        public CommunicationMediumCode? Medium { get; set; }
        public CommunicationChannelCodeC0? Code { get; set; }
    }
}

