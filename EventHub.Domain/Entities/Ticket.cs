using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public string QrCode { get; set; } = string.Empty;       // Unique QR code string
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public string EventId { get; set; }
        public string ParticipantId { get; set; }

        // Navigation
        public Event Event { get; set; } = null!;
        public User Participant { get; set; } = null!;
    }
}