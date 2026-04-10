using EventHub.DAL.Data;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventHub.DAL.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _dbSet.FirstOrDefaultAsync(u => u.Email == email.ToLower());

        public async Task<User?> GetWithDetailsAsync(int userId) =>
            await _dbSet
                .Include(u => u.OrganizedEvents)
                .Include(u => u.Tickets)
                    .ThenInclude(t => t.Event)
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<IEnumerable<User>> GetPendingOrganizersAsync() =>
            await _dbSet
                .Where(u => u.Role == UserRole.EventOrganizer && u.AccountStatus == AccountStatus.Pending)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role) =>
            await _dbSet.Where(u => u.Role == role).ToListAsync();

        public async Task<bool> EmailExistsAsync(string email) =>
            await _dbSet.AnyAsync(u => u.Email == email.ToLower());
    }
}
