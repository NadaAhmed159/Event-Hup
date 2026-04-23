using System.Security.Claims;
using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace EventHub.API.Controllers
{
    [Route("api/event")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("{eventId}/analytics")]
        [Authorize(Roles = nameof(UserRole.EventOrganizer))]
        public async Task<IActionResult> GetEventAnalytics(string eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var analytics = await _eventService.GetEventAnalyticsForOrganizerAsync(userId, eventId);
            if (analytics == null)
                return NotFound();

            return Ok(analytics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(string id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return Ok(@event);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedEvents()
        {
            var events = await _eventService.GetApprovedEventsAsync();
            return Ok(events);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingEvents()
        {
            var events = await _eventService.GetPendingEventsAsync();
            return Ok(events);
        }

        [HttpGet("organizer/{organizerId}")]
        public async Task<IActionResult> GetEventsByOrganizer(string organizerId)
        {
            var events = await _eventService.GetEventsByOrganizerAsync(organizerId);
            return Ok(events);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetEventsByCategory(string categoryId)
        {
            var events = await _eventService.GetEventsByCategoryAsync(categoryId);
            return Ok(events);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchEvents([FromQuery] string? keyword, [FromQuery] string? venue, [FromQuery] string? categoryId, [FromQuery] DateTime? eventDate)
        {
            var events = await _eventService.SearchEventsAsync(keyword, venue, categoryId, eventDate);
            return Ok(events);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingEvents([FromQuery] int count = 10)
        {
            var events = await _eventService.GetUpcomingEventsAsync(count);
            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            try
            {
                var createdEvent = await _eventService.CreateEventAsync(newEvent);
                return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(string id, [FromBody] Event updatedEvent)
        {
            if (id != updatedEvent.Id)
            {
                return BadRequest("Event ID mismatch");
            }

            try
            {
                await _eventService.UpdateEventAsync(updatedEvent);
                return NoContent();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
