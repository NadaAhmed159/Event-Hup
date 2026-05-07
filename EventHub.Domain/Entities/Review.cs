using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class Review:BaseEntity
    {

        public string UserId { get; set; }
        public User User { get; set; }

        public string EventId { get; set; }
        public Event Event { get; set; }

        public int Rating { get; set; }
        public string? Comment { get; set; }

    }
}
