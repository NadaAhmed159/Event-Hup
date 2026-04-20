using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Review> CreateAsync(Review review, CancellationToken cancellationToken = default)
        {
            if (review.Rating < 1 || review.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            if (await _unitOfWork.Reviews.HasParticipantReviewedAsync(review.UserId, review.EventId))
                throw new InvalidOperationException("You have already reviewed this event.");

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return review;
        }

        public Task<IEnumerable<Review>> GetByEventAsync(string eventId, CancellationToken cancellationToken = default) =>
            _unitOfWork.Reviews.GetByEventAsync(eventId);

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
                throw new KeyNotFoundException("Review not found.");

            _unitOfWork.Reviews.Remove(review);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
