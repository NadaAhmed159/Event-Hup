using EventHub.BLL.Services.Interfaces;

namespace EventHub.BLL.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUserService _userService;
        private readonly IEventService _eventService;

        public AdminService(IUserService userService, IEventService eventService)
        {
            _userService = userService;
            _eventService = eventService;
        }

        public async Task ApproveOrganizerAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            await _userService.ApproveOrganizerAsync(userId);
        }

        public async Task RejectOrganizerAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            await _userService.RejectOrganizerAsync(userId);
        }

        public async Task ApproveEventAsync(string eventId, CancellationToken cancellationToken = default)
        {
            var @event = await _eventService.GetEventByIdAsync(eventId);
            if (@event == null)
                throw new KeyNotFoundException("Event not found.");

            await _eventService.ApproveEventAsync(eventId);
        }

        public async Task RejectEventAsync(string eventId, CancellationToken cancellationToken = default)
        {
            var @event = await _eventService.GetEventByIdAsync(eventId);
            if (@event == null)
                throw new KeyNotFoundException("Event not found.");

            await _eventService.RejectEventAsync(eventId);
        }
    }
}
