using EventHub.API.Hubs;
using EventHub.API.Security;
using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EventHub.API.Controllers
{
    [Route("api/notification")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<EventAvailabilityHub> _hubContext;

        public NotificationController(
            INotificationService notificationService,
            IHubContext<EventAvailabilityHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var callerId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(callerId))
                return Unauthorized();

            if (!EventManagementAuth.IsAdmin(User) && callerId != userId)
                return Forbid();

            var items = await _notificationService.GetByUserAsync(userId);
            return Ok(items);
        }

        [HttpPost("send")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Send([FromBody] Notification notification)
        {
            var created = await _notificationService.SendAsync(notification);
            await _hubContext.Clients.Group(EventAvailabilityHub.UserGroup(created.UserId))
                .SendAsync("NotificationCreated", new { created.Id, created.UserId, created.Title, created.Message, created.IsRead, created.EventId, created.CreatedAt });
            return Ok(created);
        }

        [HttpPut("{id}/read")]
        [Authorize(Roles = nameof(UserRole.Participant))]
        public async Task<IActionResult> MarkRead(string id)
        {
            var callerId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(callerId))
                return Unauthorized();

            try
            {
                await _notificationService.MarkAsReadAsync(id, callerId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
