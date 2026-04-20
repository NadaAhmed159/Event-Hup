using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<EventAttachment> UploadForEventAsync(string eventId, Stream fileStream, string originalFileName, CancellationToken cancellationToken = default);
        Task<IEnumerable<EventAttachment>> GetByEventAsync(string eventId, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
