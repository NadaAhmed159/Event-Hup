namespace EventHub.Domain.Analytics
{
    /// <summary>
    /// Sales metrics for a single event (tickets sold × event unit price = total revenue).
    /// </summary>
    public class EventAnalytics
    {
        public string EventId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int TicketsSold { get; set; }
        public decimal TicketUnitPrice { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
