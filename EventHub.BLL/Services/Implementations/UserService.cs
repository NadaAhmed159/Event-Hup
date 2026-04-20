using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;

namespace EventHub.BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ApproveOrganizerAsync(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user != null && user.ApplyAs == UserRole.EventOrganizer)
            {
                user.Status = AccountStatus.Approved;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user != null)
            {
                _unitOfWork.Users.Remove(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _unitOfWork.Users.EmailExistsAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork.Users.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetPendingOrganizersAsync()
        {
            return await _unitOfWork.Users.GetPendingOrganizersAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email);
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _unitOfWork.Users.GetWithDetailsAsync(userId);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _unitOfWork.Users.GetByRoleAsync(role);
        }

        public async Task RejectOrganizerAsync(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user != null && user.ApplyAs == UserRole.EventOrganizer)
            {
                user.Status = AccountStatus.Rejected;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
