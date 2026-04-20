using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Favorite>> GetByUserAsync(string userId) =>
            await _dbSet
                .Where(w => w.UserId == userId)
                .Include(w => w.Event)
                    .ThenInclude(e => e.Category)
                .Include(w => w.Event)
                    .ThenInclude(e => e.Organizer)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

        public async Task<Favorite?> GetByUserAndEventAsync(string userId, string eventId) =>
            await _dbSet.FirstOrDefaultAsync(w =>
                w.UserId == userId && w.EventId == eventId);

        public async Task<bool> IsEventSavedAsync(string userId, string eventId) =>
            await _dbSet.AnyAsync(w =>
                w.UserId == userId && w.EventId == eventId);
    }
}
