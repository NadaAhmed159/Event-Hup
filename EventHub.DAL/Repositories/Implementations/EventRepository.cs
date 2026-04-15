using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context) { }

        public async Task<Event?> GetWithDetailsAsync(string eventId) =>
            await _dbSet
                .Include(e => e.Organizer)
                .Include(e => e.Category)
                .Include(e => e.Reviews)
                .Include(e => e.Attachments)
                .FirstOrDefaultAsync(e => e.Id == eventId);

        public async Task<IEnumerable<Event>> GetApprovedEventsAsync() =>
            await _dbSet
                .Where(e => e.Status == EventStatus.Approved)
                .Include(e => e.Organizer)
                .Include(e => e.Category)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

        public async Task<IEnumerable<Event>> GetPendingEventsAsync() =>
            await _dbSet
                .Where(e => e.Status == EventStatus.Pending)
                .Include(e => e.Organizer)
                .Include(e => e.Category)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Event>> GetByOrganizerAsync(string organizerId) =>
            await _dbSet
                .Where(e => e.OrganizerId == organizerId)
                .Include(e => e.Category)
                .Include(e => e.Tickets)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Event>> SearchEventsAsync(
            string? keyword,
            string? venue,
            string? categoryId,
            DateTime? eventDate)
        {
            var query = _dbSet
                .Where(e => e.Status == EventStatus.Approved)
                .Include(e => e.Organizer)
                .Include(e => e.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(e =>
                    e.Title.ToLower().Contains(keyword.ToLower()) ||
                    e.Description.ToLower().Contains(keyword.ToLower()) ||
                    e.Venue.ToLower().Contains(keyword.ToLower()));

            if (!string.IsNullOrWhiteSpace(venue))
                query = query.Where(e => e.Venue != null && e.Venue.ToLower().Contains(venue.ToLower()));

            if (!string.IsNullOrWhiteSpace(categoryId))
                query = query.Where(e => e.CategoryId == categoryId);

            if (eventDate.HasValue)
                query = query.Where(e => e.EventDate >= eventDate.Value);

            return await query.OrderBy(e => e.EventDate).ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(string categoryId) =>
            await _dbSet
                .Where(e => e.CategoryId == categoryId && e.Status == EventStatus.Approved)
                .Include(e => e.Organizer)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        public async Task<int> GetSoldTicketsCountAsync(string eventId) =>
            await _dbSet
                .Where(e => e.Id == eventId)
                .Select(e => e.TotalTickets - e.AvailableTickets)
                .FirstOrDefaultAsync();

        public async Task<int> GetAvailableTicketsCountAsync(string eventId) =>
            await _dbSet
                .Where(e => e.Id == eventId)
                .Select(e => e.AvailableTickets)
                .FirstOrDefaultAsync();

        // public async Task<(int ticketsSold, decimal totalRevenue)> GetEventAnalyticsAsync(int eventId)
        // {
        //     var tickets = await _context.Tickets
        //         .Where(t => t.EventId == eventId && t.Status != TicketStatus.Cancelled && t.Status != TicketStatus.Refunded)
        //         .ToListAsync();

        //     return (tickets.Count, tickets.Sum(t => t.PricePaid));
        // }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count) =>
            await _dbSet
                .Where(e => e.Status == EventStatus.Approved && e.EventDate > DateTime.UtcNow)
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .OrderBy(e => e.EventDate)
                .Take(count)
                .ToListAsync();
    }
}
