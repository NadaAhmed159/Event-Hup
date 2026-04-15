using EventHub.Domain.Entities;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetByEventAsync(string eventId);
        Task<Review?> GetByParticipantAndEventAsync(string participantId, string eventId);
        Task<double> GetAverageRatingAsync(string eventId);
        Task<bool> HasParticipantReviewedAsync(string participantId, string eventId);
    }
}
