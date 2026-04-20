using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Entities;
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
        public async Task<IActionResult> Create([FromBody] Review review)
        {
            try
            {
                var created = await _reviewService.CreateAsync(review);
                return CreatedAtAction(nameof(GetByEvent), new { eventId = created.EventId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetByEvent(string eventId)
        {
            var reviews = await _reviewService.GetByEventAsync(eventId);
            return Ok(reviews);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _reviewService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
