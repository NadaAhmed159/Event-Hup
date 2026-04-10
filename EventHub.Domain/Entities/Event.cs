using EventHub.Domain.Common;
using EventHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class Event:BaseEntity
    {
        public string OrganizerId { get; set; }
        public User Organizer { get; set; }
        public string CategoryId { get; set; }
        public Category Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Venue { get; set; }
        public DateTime EventDate { get; set; }
        public string? Image { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Pending;
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }

        // Navigation
        public ICollection<Ticket> Tickets { get; set; }=new List<Ticket>();
        public ICollection<Review> Reviews { get; set; }= new List<Review>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<EventAttachment> Attachments { get; set; } = new List<EventAttachment>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
