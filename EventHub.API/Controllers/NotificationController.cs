using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var items = await _notificationService.GetByUserAsync(userId);
            return Ok(items);
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] Notification notification)
        {
            var created = await _notificationService.SendAsync(notification);
            return Ok(created);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkRead(string id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
