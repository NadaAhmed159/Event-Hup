using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order?> GetByIdAsync(string orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetMyOrdersAsync(string participantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByEventAsync(string eventId, CancellationToken cancellationToken = default);
    }
}
