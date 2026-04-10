using EventHub.Domain.Entities;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId);
        Task<Favorite?> GetUserFavoriteAsync(int userId, int eventId);
        Task<bool> IsFavoritedAsync(int userId, int eventId);
    }

    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetEventReviewsAsync(int eventId);
        Task<Review?> GetUserReviewForEventAsync(int userId, int eventId);
        Task<double> GetAverageRatingAsync(int eventId);
    }

    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task MarkAllAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }

    public interface IEventAttachmentRepository : IGenericRepository<EventAttachment>
    {
        Task<IEnumerable<EventAttachment>> GetAttachmentsByEventAsync(int eventId);
    }
}
