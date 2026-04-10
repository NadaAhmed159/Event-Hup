using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.DAL.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetWithDetailsAsync(int userId);
        Task<IEnumerable<User>> GetPendingOrganizersAsync();
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
        Task<bool> EmailExistsAsync(string email);
    }
}
