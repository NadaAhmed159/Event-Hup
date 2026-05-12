using EventHub.API.Security;
using EventHub.BLL.Models;
using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [Route("api/review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Participant))]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            var userId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var review = new Review
            {
                UserId = userId,
                EventId = request.EventId,
                Rating = request.Rating,
                Comment = request.Comment
            };

            try
            {
                var created = await _reviewService.CreateAsync(review);
                return CreatedAtAction(nameof(GetByEvent), new { eventId = created.EventId }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [HttpGet("event/{eventId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByEvent(string eventId)
        {
            var reviews = await _reviewService.GetByEventAsync(eventId);
            return Ok(reviews);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Participant))]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = EventManagementAuth.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await _reviewService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

    }
}
