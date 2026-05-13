using System.Collections.Generic;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<EventAttachment?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<(EventAttachment Attachment, IReadOnlyList<Notification> Notifications)> UploadForEventAsync(string eventId, Stream fileStream, string originalFileName, CancellationToken cancellationToken = default);
        Task<IEnumerable<EventAttachment>> GetByEventAsync(string eventId, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
