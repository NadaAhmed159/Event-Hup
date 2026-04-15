using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserAsync(string userId, bool unreadOnly = false)
        {
            var query = _dbSet.Where(n => n.UserId == userId);
            if (unreadOnly)
                query = query.Where(n => !n.IsRead);
            return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId) =>
            await _dbSet.CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task MarkAsReadAsync(string notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
            }
        }

        public async Task<IEnumerable<Notification>> GetByEventAsync(string eventId) =>
            await _dbSet
                .Where(n => n.EventId == eventId)
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
    }
}
