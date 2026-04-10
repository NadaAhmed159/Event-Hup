using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket?> GetByQrCodeAsync(string qrCode);
        Task<Ticket?> GetByUniqueCodeAsync(string uniqueCode);
        Task<Ticket?> GetWithDetailsAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetByParticipantAsync(int participantId);
        Task<IEnumerable<Ticket>> GetByEventAsync(int eventId);
        Task<bool> HasParticipantPurchasedAsync(int participantId, int eventId);
        Task<int> GetSoldTicketsCountAsync(int eventId);
        Task<decimal> GetTotalRevenueAsync(int eventId);
    }
}
