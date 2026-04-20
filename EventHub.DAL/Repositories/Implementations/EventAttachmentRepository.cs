using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class EventAttachmentRepository : GenericRepository<EventAttachment>, IEventAttachmentRepository
    {
        public EventAttachmentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<EventAttachment>> GetByEventAsync(string eventId) =>
            await _dbSet
                .Where(a => a.EventId == eventId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
    }
}
