using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<Notification>> GetByUserAsync(string userId, CancellationToken cancellationToken = default) =>
            _unitOfWork.Notifications.GetByUserAsync(userId);

        public async Task<Notification> SendAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            return notification;
        }

        public async Task MarkAsReadAsync(string id, CancellationToken cancellationToken = default)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (notification == null)
                throw new KeyNotFoundException("Notification not found.");

            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
