using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        Task<Event?> GetWithDetailsAsync(string eventId);
        Task<IEnumerable<Event>> GetApprovedEventsAsync();
        Task<IEnumerable<Event>> GetPendingEventsAsync();
        Task<IEnumerable<Event>> GetByOrganizerAsync(string organizerId);
        Task<IEnumerable<Event>> SearchEventsAsync(
            string? keyword,
            string? venue,
            string? categoryId,
            DateTime? eventDate);
        Task<IEnumerable<Event>> GetEventsByCategoryAsync(string categoryId);
        Task<int> GetSoldTicketsCountAsync(string eventId);
        Task<int> GetAvailableTicketsCountAsync(string eventId);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count);
    }
}
