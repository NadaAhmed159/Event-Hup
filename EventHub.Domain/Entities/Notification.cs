using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class Notification:BaseEntity
    {

        public string UserId { get; set; }
        public User User { get; set; }

        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public string Title { get; set; } = string.Empty;


        public string? EventId { get; set; }     // Optional — some notifications aren't event-related
        public Event? Event { get; set; }

    }
}
