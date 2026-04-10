using EventHub.Domain.Entities;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IWatchlistRepository : IGenericRepository<WatchlistItem>
    {
        Task<IEnumerable<WatchlistItem>> GetByUserAsync(int userId);
        Task<WatchlistItem?> GetByUserAndEventAsync(int userId, int eventId);
        Task<bool> IsEventSavedAsync(int userId, int eventId);
    }
}
