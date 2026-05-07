using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Order?> GetByIdAsync(string orderId, CancellationToken cancellationToken = default) =>
            _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);

        public Task<IEnumerable<Order>> GetMyOrdersAsync(string participantId, CancellationToken cancellationToken = default) =>
            _unitOfWork.Orders.GetByParticipantAsync(participantId);

        public Task<IEnumerable<Order>> GetByEventAsync(string eventId, CancellationToken cancellationToken = default) =>
            _unitOfWork.Orders.GetByEventAsync(eventId);
    }
}
