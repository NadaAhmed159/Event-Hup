using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class WatchlistRepository : GenericRepository<WatchlistItem>, IWatchlistRepository
    {
        public WatchlistRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<WatchlistItem>> GetByUserAsync(int userId) =>
            await _dbSet
                .Where(w => w.UserId == userId)
                .Include(w => w.Event)
                    .ThenInclude(e => e.Category)
                .Include(w => w.Event)
                    .ThenInclude(e => e.Organizer)
                .OrderByDescending(w => w.SavedAt)
                .ToListAsync();

        public async Task<WatchlistItem?> GetByUserAndEventAsync(int userId, int eventId) =>
            await _dbSet.FirstOrDefaultAsync(w =>
                w.UserId == userId && w.EventId == eventId);

        public async Task<bool> IsEventSavedAsync(int userId, int eventId) =>
            await _dbSet.AnyAsync(w =>
                w.UserId == userId && w.EventId == eventId);
    }
}
