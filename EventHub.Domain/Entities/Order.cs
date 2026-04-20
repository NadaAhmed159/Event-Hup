using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Domain.Common;

namespace EventHub.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ParticipantId { get; set; }
        public User Participant { get; set; }

        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
