using EventHub.API.Security;
using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [Route("api/favorite")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("event/{eventId}")]
        [Authorize(Roles = nameof(UserRole.Participant))]
        public async Task<IActionResult> Add(string eventId)
        {
            var userId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var favorite = new Favorite
            {
                UserId = userId,
                EventId = eventId
            };

            try
            {
                var created = await _favoriteService.AddAsync(favorite);
                return Created($"/api/favorite/{created.Id}", created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Participant))]
        public async Task<IActionResult> Remove(string id)
        {
            var userId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await _favoriteService.RemoveAsync(id, userId);
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

        [HttpGet("user/{userId}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var items = await _favoriteService.GetByUserAsync(userId);
            return Ok(items);
        }
    }
}
