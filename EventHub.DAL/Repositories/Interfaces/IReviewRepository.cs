using EventHub.Domain.Entities;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetByEventAsync(int eventId);
        Task<Review?> GetByParticipantAndEventAsync(int participantId, int eventId);
        Task<double> GetAverageRatingAsync(int eventId);
        Task<bool> HasParticipantReviewedAsync(int participantId, int eventId);
    }
}
