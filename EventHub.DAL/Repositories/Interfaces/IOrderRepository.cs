using EventHub.Domain.Entities;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetByIdWithDetailsAsync(string orderId);
        Task<IEnumerable<Order>> GetByParticipantAsync(string participantId);
        Task<IEnumerable<Order>> GetByEventAsync(string eventId);
    }
}
