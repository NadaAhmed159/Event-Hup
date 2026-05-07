using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetByEventAsync(string eventId) =>
            await _dbSet
                .Where(r => r.EventId == eventId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Review?> GetByParticipantAndEventAsync(string participantId, string eventId) =>
            await _dbSet.FirstOrDefaultAsync(r =>
                r.UserId == participantId && r.EventId == eventId);

        public async Task<double> GetAverageRatingAsync(string eventId)
        {
            var ratings = await _dbSet
                .Where(r => r.EventId == eventId)
                .Select(r => r.Rating)
                .ToListAsync();

            return ratings.Count == 0 ? 0 : ratings.Average();
        }

        public async Task<bool> HasParticipantReviewedAsync(string participantId, string eventId) =>
            await _dbSet.AnyAsync(r =>
                r.UserId == participantId && r.EventId == eventId);
    }
}
