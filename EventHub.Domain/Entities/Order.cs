using EventHub.Domain.Common;

namespace EventHub.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string ParticipantId { get; set; }
        public User Participant { get; set; } = null!;

        public string EventId { get; set; } = string.Empty;
        public Event Event { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
