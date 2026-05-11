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
        /// <summary>Relative path (from app content root, forward slashes) to a PNG that encodes <see cref="QrCode"/>.</summary>
        public string? QrCodeImagePath { get; set; }
        /// <summary>When set, the ticket was verified at check-in (QR verify endpoint). Subsequent verifications are rejected.</summary>
        public DateTime? UsedAtUtc { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public string EventId { get; set; }
        public string ParticipantId { get; set; }
        public string? OrderId { get; set; }

        // Navigation
        public Event Event { get; set; } = null!;
        public User Participant { get; set; } = null!;
        public Order? Order { get; set; }
    }
}