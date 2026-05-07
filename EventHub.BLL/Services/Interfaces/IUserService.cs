using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetPendingOrganizersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<bool> EmailExistsAsync(string email);

        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string userId);

        Task ApproveOrganizerAsync(string userId);
        Task RejectOrganizerAsync(string userId);
    }
}
