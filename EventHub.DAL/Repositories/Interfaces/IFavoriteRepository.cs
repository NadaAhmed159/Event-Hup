using EventHub.Domain.Entities;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<IEnumerable<Favorite>> GetByUserAsync(string userId);
        Task<Favorite?> GetByUserAndEventAsync(string userId, string eventId);
        Task<bool> IsEventSavedAsync(string userId, string eventId);
    }
}