using EventHub.BLL.Mapping;
using EventHub.BLL.Services.Interfaces;
using EventHub.API.Security;
using EventHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [Route("api/attachment")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IEventService _eventService;

        public AttachmentController(IAttachmentService attachmentService, IEventService eventService)
        {
            _attachmentService = attachmentService;
            _eventService = eventService;
        }

        [HttpPost("upload")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.EventOrganizer)}")]
        [RequestSizeLimit(52_428_800)]
        public async Task<IActionResult> Upload([FromQuery] string eventId, IFormFile file, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest("eventId is required.");

            if (file == null || file.Length == 0)
                return BadRequest("A file is required.");

            var userId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var @event = await _eventService.GetEventByIdAsync(eventId);
            if (@event == null)
                return NotFound();

            if (!EventManagementAuth.CanMutateEvent(User, @event.OrganizerId))
                return Forbid();

            try
            {
                await using var stream = file.OpenReadStream();
                var attachment = await _attachmentService.UploadForEventAsync(eventId, stream, file.FileName, cancellationToken);
                return Ok(EventDtoMapper.ToAttachmentResponseDto(attachment));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetByEvent(string eventId)
        {
            var items = await _attachmentService.GetByEventAsync(eventId);
            return Ok(EventDtoMapper.ToAttachmentResponseDtos(items));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.EventOrganizer)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var attachment = await _attachmentService.GetByIdAsync(id);
            if (attachment == null)
                return NotFound();

            var @event = await _eventService.GetEventByIdAsync(attachment.EventId);
            if (@event == null)
                return NotFound();

            if (!EventManagementAuth.CanMutateEvent(User, @event.OrganizerId))
                return Forbid();

            try
            {
                await _attachmentService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
