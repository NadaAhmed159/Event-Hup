using EventHub.BLL.Services.Interfaces;
using EventHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("organizers/{id}/approve")]
        public async Task<IActionResult> ApproveOrganizer(string id)
        {
            try
            {
                await _adminService.ApproveOrganizerAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("organizers/{id}/reject")]
        public async Task<IActionResult> RejectOrganizer(string id)
        {
            try
            {
                await _adminService.RejectOrganizerAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("events/{id}/approve")]
        public async Task<IActionResult> ApproveEvent(string id)
        {
            try
            {
                await _adminService.ApproveEventAsync(id);
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

        [HttpPost("events/{id}/reject")]
        public async Task<IActionResult> RejectEvent(string id)
        {
            try
            {
                await _adminService.RejectEventAsync(id);
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
