using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Domain.Entities;

namespace EventHub.BLL.Services.Interfaces
{
    public interface IEventService
    {
        Task<Event?> GetEventByIdAsync(string eventId);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<IEnumerable<Event>> GetApprovedEventsAsync();
        Task<IEnumerable<Event>> GetPendingEventsAsync();
        Task<IEnumerable<Event>> GetEventsByOrganizerAsync(string organizerId);
        Task<IEnumerable<Event>> GetEventsByCategoryAsync(string categoryId);
        Task<IEnumerable<Event>> SearchEventsAsync(string? keyword, string? venue, string? categoryId, DateTime? eventDate);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count);
        
        Task<Event> CreateEventAsync(Event newEvent);
        Task UpdateEventAsync(Event updatedEvent);
        Task DeleteEventAsync(string eventId);
        
        Task ApproveEventAsync(string eventId);
        Task RejectEventAsync(string eventId);
    }
}
