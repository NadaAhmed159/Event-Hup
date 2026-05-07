using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<Notification> SendAsync(Notification notification, CancellationToken cancellationToken = default);
        Task MarkAsReadAsync(string id, CancellationToken cancellationToken = default);
    }
}
