using EventHub.Domain.Entities;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IEventAttachmentRepository : IGenericRepository<EventAttachment>
    {
        Task<IEnumerable<EventAttachment>> GetByEventAsync(int eventId);
    }
}
