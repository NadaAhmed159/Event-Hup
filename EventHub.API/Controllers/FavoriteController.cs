using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Entities;
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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Favorite favorite)
        {
            try
            {
                var created = await _favoriteService.AddAsync(favorite);
                return CreatedAtAction(nameof(GetByUser), new { userId = created.UserId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            try
            {
                await _favoriteService.RemoveAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var items = await _favoriteService.GetByUserAsync(userId);
            return Ok(items);
        }
    }
}
