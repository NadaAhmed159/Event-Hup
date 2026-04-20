using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<Favorite> AddAsync(Favorite favorite, CancellationToken cancellationToken = default);
        Task RemoveAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Favorite>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    }
}
