using EventHub.Domain.Entities;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket?> GetByQrCodeAsync(string qrCode);
        Task<Ticket?> GetByQrCodeWithDetailsAsync(string qrCode, CancellationToken cancellationToken = default);
        Task<Ticket?> GetWithDetailsAsync(string ticketId);
        Task<IEnumerable<Ticket>> GetByParticipantAsync(string participantId);
        Task<IEnumerable<Ticket>> GetByEventAsync(string eventId);
        Task<bool> HasParticipantPurchasedAsync(string participantId, string eventId);
        Task<IReadOnlyList<string>> GetDistinctParticipantIdsForEventAsync(string eventId, CancellationToken cancellationToken = default);
    }
}
