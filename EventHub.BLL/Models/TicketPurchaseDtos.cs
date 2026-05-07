namespace EventHub.BLL.Models
{
    public sealed class TicketPurchaseResult
    {
        public string OrderId { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string ParticipantId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int RemainingAvailableTickets { get; set; }
        public IReadOnlyList<string> TicketIds { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> TicketQrCodes { get; set; } = Array.Empty<string>();
    }
}
