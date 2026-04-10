using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class OrderItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int Quantity { get; set; }

        // Navigation
        public ICollection<DigitalTicket> DigitalTickets { get; set; }
    }
}
