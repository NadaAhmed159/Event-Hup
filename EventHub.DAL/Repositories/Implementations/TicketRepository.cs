using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(AppDbContext context) : base(context) { }

        public async Task<Ticket?> GetByQrCodeAsync(string qrCode) =>
            await _dbSet.FirstOrDefaultAsync(t => t.QrCode == qrCode);

        public async Task<Ticket?> GetByUniqueCodeAsync(string uniqueCode) =>
            await _dbSet.FirstOrDefaultAsync(t => t.UniqueCode == uniqueCode);

        public async Task<Ticket?> GetWithDetailsAsync(int ticketId) =>
            await _dbSet
                .Include(t => t.Event)
                    .ThenInclude(e => e.Organizer)
                .Include(t => t.Participant)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

        public async Task<IEnumerable<Ticket>> GetByParticipantAsync(int participantId) =>
            await _dbSet
                .Where(t => t.ParticipantId == participantId)
                .Include(t => t.Event)
                    .ThenInclude(e => e.Category)
                .OrderByDescending(t => t.PurchasedAt)
                .ToListAsync();

        public async Task<IEnumerable<Ticket>> GetByEventAsync(int eventId) =>
            await _dbSet
                .Where(t => t.EventId == eventId)
                .Include(t => t.Participant)
                .OrderByDescending(t => t.PurchasedAt)
                .ToListAsync();

        public async Task<bool> HasParticipantPurchasedAsync(int participantId, int eventId) =>
            await _dbSet.AnyAsync(t =>
                t.ParticipantId == participantId &&
                t.EventId == eventId &&
                t.Status == TicketStatus.Active);

        public async Task<int> GetSoldTicketsCountAsync(int eventId) =>
            await _dbSet.CountAsync(t =>
                t.EventId == eventId &&
                t.Status != TicketStatus.Cancelled &&
                t.Status != TicketStatus.Refunded);

        public async Task<decimal> GetTotalRevenueAsync(int eventId) =>
            await _dbSet
                .Where(t => t.EventId == eventId &&
                            t.Status != TicketStatus.Cancelled &&
                            t.Status != TicketStatus.Refunded)
                .SumAsync(t => t.PricePaid);
    }
}
