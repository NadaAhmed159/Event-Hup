using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Analytics;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.BLL.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ApproveEventAsync(string eventId)
        {
            var eventObj = await _unitOfWork.Events.GetByIdAsync(eventId);
            if (eventObj != null)
            {
                eventObj.Status = EventStatus.Approved;
                _unitOfWork.Events.Update(eventObj);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            newEvent.Status = EventStatus.Pending;
            await _unitOfWork.Events.AddAsync(newEvent);
            await _unitOfWork.SaveChangesAsync();
            return newEvent;
        }

        public async Task DeleteEventAsync(string eventId)
        {
            var eventObj = await _unitOfWork.Events.GetByIdAsync(eventId);
            if (eventObj != null)
            {
                _unitOfWork.Events.Remove(eventObj);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _unitOfWork.Events.GetAllAsync();
        }

        public async Task<IEnumerable<Event>> GetApprovedEventsAsync()
        {
            return await _unitOfWork.Events.GetApprovedEventsAsync();
        }

        public async Task<Event?> GetEventByIdAsync(string eventId)
        {
            return await _unitOfWork.Events.GetWithDetailsAsync(eventId);
        }

        public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(string categoryId)
        {
            return await _unitOfWork.Events.GetEventsByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Event>> GetEventsByOrganizerAsync(string organizerId)
        {
            return await _unitOfWork.Events.GetByOrganizerAsync(organizerId);
        }

        public async Task<IEnumerable<Event>> GetPendingEventsAsync()
        {
            return await _unitOfWork.Events.GetPendingEventsAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count)
        {
            return await _unitOfWork.Events.GetUpcomingEventsAsync(count);
        }

        public async Task RejectEventAsync(string eventId)
        {
            var eventObj = await _unitOfWork.Events.GetByIdAsync(eventId);
            if (eventObj != null)
            {
                eventObj.Status = EventStatus.Rejected; // Or a specific Rejected status if available
                _unitOfWork.Events.Update(eventObj);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Event>> SearchEventsAsync(string? keyword, string? venue, string? categoryId, DateTime? eventDate)
        {
            return await _unitOfWork.Events.SearchEventsAsync(keyword, venue, categoryId, eventDate);
        }

        public async Task UpdateEventAsync(Event updatedEvent)
        {
            _unitOfWork.Events.Update(updatedEvent);
            await _unitOfWork.SaveChangesAsync();
        }

        public Task<EventAnalytics?> GetEventAnalyticsForOrganizerAsync(string organizerUserId, string eventId) =>
            _unitOfWork.Events.GetAnalyticsForOrganizerEventAsync(eventId, organizerUserId);
    }
}
