using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class EventAttachment:BaseEntity
    {

        public string EventId { get; set; }
        public Event Event { get; set; }

        public string FilePath { get; set; }
    }
}
