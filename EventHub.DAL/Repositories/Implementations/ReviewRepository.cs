using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetByEventAsync(int eventId) =>
            await _dbSet
                .Where(r => r.EventId == eventId)
                .Include(r => r.Participant)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Review?> GetByParticipantAndEventAsync(int participantId, int eventId) =>
            await _dbSet.FirstOrDefaultAsync(r =>
                r.ParticipantId == participantId && r.EventId == eventId);

        public async Task<double> GetAverageRatingAsync(int eventId)
        {
            var ratings = await _dbSet
                .Where(r => r.EventId == eventId)
                .Select(r => r.Rating)
                .ToListAsync();

            return ratings.Count == 0 ? 0 : ratings.Average();
        }

        public async Task<bool> HasParticipantReviewedAsync(int participantId, int eventId) =>
            await _dbSet.AnyAsync(r =>
                r.ParticipantId == participantId && r.EventId == eventId);
    }
}
