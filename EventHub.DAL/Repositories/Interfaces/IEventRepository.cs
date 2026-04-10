using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        Task<Event?> GetWithDetailsAsync(int eventId);
        Task<IEnumerable<Event>> GetApprovedEventsAsync();
        Task<IEnumerable<Event>> GetPendingEventsAsync();
        Task<IEnumerable<Event>> GetByOrganizerAsync(int organizerId);
        Task<IEnumerable<Event>> SearchEventsAsync(
            string? keyword,
            string? location,
            int? categoryId,
            DateTime? fromDate,
            DateTime? toDate,
            decimal? minPrice,
            decimal? maxPrice);
        Task<IEnumerable<Event>> GetEventsByCategoryAsync(int categoryId);
        Task<(int ticketsSold, decimal totalRevenue)> GetEventAnalyticsAsync(int eventId);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count);
    }
}
