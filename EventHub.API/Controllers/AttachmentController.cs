using EventHub.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.API.Controllers
{
    [Route("api/attachment")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(52_428_800)]
        public async Task<IActionResult> Upload([FromQuery] string eventId, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("A file is required.");

            try
            {
                await using var stream = file.OpenReadStream();
                var attachment = await _attachmentService.UploadForEventAsync(eventId, stream, file.FileName, cancellationToken);
                return Ok(attachment);
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
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
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
