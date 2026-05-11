using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

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

        public async Task MarkAsReadAsync(string id, string participantUserId, CancellationToken cancellationToken = default)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (notification == null)
                throw new KeyNotFoundException("Notification not found.");

            if (notification.UserId != participantUserId)
                throw new UnauthorizedAccessException("You can only mark your own notifications as read.");

            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task NotifyApprovedParticipantsNewEventCreatedAsync(string eventId, string eventTitle, CancellationToken cancellationToken = default)
        {
            var participantIds = await _unitOfWork.Users.GetApprovedUserIdsByRoleAsync(UserRole.Participant);
            if (participantIds.Count == 0)
                return;

            const string title = "New event created";
            var safeTitle = string.IsNullOrWhiteSpace(eventTitle) ? "Untitled event" : eventTitle.Trim();
            var message = $"An event organizer submitted a new event: {safeTitle}. It is pending approval.";

            var notifications = participantIds.Select(userId => new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                EventId = eventId,
                IsRead = false
            }).ToList();

            await _unitOfWork.Notifications.AddRangeAsync(notifications);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task NotifyTicketHoldersOfNewEventAttachmentAsync(string eventId, string uploadedFileDisplayName, CancellationToken cancellationToken = default)
        {
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
            if (eventEntity == null)
                return;

            if (eventEntity.Status != EventStatus.Approved || eventEntity.EventDate <= DateTime.UtcNow)
                return;

            var participantIds = await _unitOfWork.Tickets.GetDistinctParticipantIdsForEventAsync(eventId, cancellationToken);
            if (participantIds.Count == 0)
                return;

            const string title = "New attachment for your event";
            var safeEventTitle = string.IsNullOrWhiteSpace(eventEntity.Title) ? "your event" : eventEntity.Title.Trim();
            var safeFile = string.IsNullOrWhiteSpace(uploadedFileDisplayName) ? "a new file" : uploadedFileDisplayName.Trim();
            var message = $"A new file was added to \"{safeEventTitle}\": {safeFile}.";

            var notifications = participantIds.Select(userId => new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                EventId = eventId,
                IsRead = false
            }).ToList();

            await _unitOfWork.Notifications.AddRangeAsync(notifications);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
