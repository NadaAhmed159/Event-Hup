namespace EventHub.BLL.Models
{
    public sealed class OrderEventDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Venue { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Image { get; set; }
    }

    public sealed class OrderTicketDto
    {
        public string Id { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public string? QrCodeImagePath { get; set; }
        public DateTime? UsedAtUtc { get; set; }
        public DateTime PurchasedAt { get; set; }
    }

    public sealed class OrderDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string ParticipantId { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        public OrderEventDto? Event { get; set; }
        public List<OrderTicketDto> Tickets { get; set; } = new();
    }
}

