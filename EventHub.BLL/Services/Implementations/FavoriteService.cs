using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoriteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Favorite> AddAsync(Favorite favorite, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Favorites.IsEventSavedAsync(favorite.UserId, favorite.EventId))
                throw new InvalidOperationException("Event is already on the watchlist.");

            await _unitOfWork.Favorites.AddAsync(favorite);
            await _unitOfWork.SaveChangesAsync();
            return favorite;
        }

        public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdAsync(id);
            if (favorite == null)
                throw new KeyNotFoundException("Watchlist entry not found.");

            _unitOfWork.Favorites.Remove(favorite);
            await _unitOfWork.SaveChangesAsync();
        }

        public Task<IEnumerable<Favorite>> GetByUserAsync(string userId, CancellationToken cancellationToken = default) =>
            _unitOfWork.Favorites.GetByUserAsync(userId);
    }
}
