namespace EventHub.BLL.Models
{
    public sealed class TicketPurchaseResult
    {
        public string OrderId { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string ParticipantId { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public int RemainingAvailableTickets { get; set; }
        public string TicketId { get; set; } = string.Empty;
        public string TicketQrCode { get; set; } = string.Empty;
        public string TicketQrCodeImagePath { get; set; } = string.Empty;
    }
}
