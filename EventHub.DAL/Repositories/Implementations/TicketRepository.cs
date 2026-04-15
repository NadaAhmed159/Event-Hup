using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(AppDbContext context) : base(context) { }

        public async Task<Ticket?> GetByQrCodeAsync(string qrCode) =>
            await _dbSet.FirstOrDefaultAsync(t => t.QrCode == qrCode);

        public async Task<Ticket?> GetWithDetailsAsync(string ticketId) =>
            await _dbSet
                .Include(t => t.Event)
                    .ThenInclude(e => e.Organizer)
                .Include(t => t.Participant)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

        public async Task<IEnumerable<Ticket>> GetByParticipantAsync(string participantId) =>
            await _dbSet
                .Where(t => t.ParticipantId == participantId)
                .Include(t => t.Event)
                    .ThenInclude(e => e.Category)
                .OrderByDescending(t => t.PurchasedAt)
                .ToListAsync();

        public async Task<IEnumerable<Ticket>> GetByEventAsync(string eventId) =>
            await _dbSet
                .Where(t => t.EventId == eventId)
                .Include(t => t.Participant)
                .OrderByDescending(t => t.PurchasedAt)
                .ToListAsync();

        public async Task<bool> HasParticipantPurchasedAsync(string participantId, string eventId) =>
            await _dbSet.AnyAsync(t =>
                t.ParticipantId == participantId &&
                t.EventId == eventId);
    }
}
